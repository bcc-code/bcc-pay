using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Infrastructure.Configuration;
using BccPay.Core.Sample.Mappers;
using BccPay.Core.Sample.Middleware;
using BccPay.Core.Sample.Validation;
using BccPay.Core.Shared.Helpers;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(opt => opt.ValidateScopes = false);

var configuration = builder.Configuration;
var services = builder.Services;

var bccPaymentsConfiguration = configuration.GetSection("BccPaymentsConfiguration");
services.Configure<BccPaymentsConfiguration>(bccPaymentsConfiguration);

services.AddRavenDatabaseDocumentStore();
services.AddDocumentStoreListener();

services.ConfigureBccPayInfrastructureServices(bccPay =>
{
    bccPay.AddCustomSettings(options =>
    {
        options.StoreCountryCodeFormat = CountryCodeFormat.Alpha2;
        options.DisplayCountryCodeFormat = CountryCodeFormat.ShortName;
    });

    bccPay.AddFixer(options => options.BaseAddress = "https://data.fixer.io");

    bccPay.AddMollie(options =>
    {
        options.BaseAddress = "https://api.mollie.com";
        options.AuthToken = configuration["MollieSecretKey"];
        options.WebhookUrl = "https://en46nkjh5kbngpp.m.pipedream.net/";
        options.RedirectUrl = "{{host}}/Payment/webhook/";
    });

    bccPay.AddNets(options =>
    {
        options.BaseAddress = "https://test.api.dibspayment.eu";
        options.CheckoutPageUrl = "/";
        options.TermsUrl = "http://localhost:8000";
        options.SecretKey = configuration["NetsSecretKey"];
        options.NotificationUrl = "https://localhost:5001/Payment/webhook/";
        options.ReturnUrl = "https://localhost:2001/Payment/webhook/";
    });
});

services.ConfigureBccCoreCqrs();

var _allowOrigins = "_allowOrigins";

services.AddCors(options =>
{
    options.AddPolicy(name: _allowOrigins,
        builder =>
        {
            builder.WithOrigins(configuration["CorsUrl"].Split(','))
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader();
        });
});

services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
services.AddMvc()
    .AddJsonOptions(op =>
    {
        op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblies(new List<Assembly>
            {typeof(CreatePaymentCommandValidator).Assembly}));

services.AddAutoMapper(typeof(PaymentProfile).Assembly);

services.AddControllers();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BccPay.Core.Sample", Version = "v1" });
});

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BccPay.Core.Sample v1"));
}

app.UseMiddleware<ErrorHandlingMiddleware>();

HttpContextHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(_allowOrigins);

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.InitRavenDatabase();

app.InitPaymentsConfiguration();

await app.RunAsync();