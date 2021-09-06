using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using BccPay.Core.Cqrs;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Infrastructure.Configuration;
using BccPay.Core.Sample.Mappers;
using BccPay.Core.Sample.Middleware;
using BccPay.Core.Sample.Validation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BccPay.Core.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private const string _allowOrigins = "_allowOrigins";

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var bccPaymentsConfiguration = Configuration.GetSection("BccPaymentsConfiguration");
            services.Configure<BccPaymentsConfiguration>(bccPaymentsConfiguration);

            services.AddRavenDatabaseDocumentStore();

            services.ConfigureBccPayInfrastructureServices(bccPay =>
            {
                bccPay.AddFixer(options => options.BaseAddress = "https://data.fixer.io");

                bccPay.AddMollie(options =>
                {
                    options.BaseAddress = "https://api.mollie.com";
                    options.AuthToken = Configuration["MollieSecretKey"];
                    options.RedirectUrl = "https://test.api.samvirk.com/redirect";
                    options.WebhookUrl = "https://en46nkjh5kbngpp.m.pipedream.net/";
                    options.RedirectUrlMobileApp = "com.ionic.samvirk://overview";
                    options.RateMarkup = 0.015M;
                });

                bccPay.AddNets(options =>
                {
                    options.BaseAddress = "https://test.api.dibspayment.eu";
                    options.CheckoutPageUrl = "/";
                    options.TermsUrl = "http://localhost:8000";
                    options.SecretKey = Configuration["NetsSecretKey"];
                    options.NotificationUrl = "https://localhost:5001/Payment/webhook/";
                });
            });

            services.ConfigureBccCoreCqrs();

            services.AddCors(options =>
            {
                options.AddPolicy(name: _allowOrigins,
                    builder =>
                    {
                        builder.WithOrigins(Configuration.GetValue<string>("CorsUrl").Split(','))
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
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(new List<Assembly> { typeof(CreatePaymentCommandValidator).Assembly }));
            services.AddAutoMapper(typeof(PaymentProfile).Assembly);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BccPay.Core.Sample", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BccPay.Core.Sample v1"));
            }
            app.UseMiddleware<ErrorHandlingMiddleware>();

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
        }
    }
}
