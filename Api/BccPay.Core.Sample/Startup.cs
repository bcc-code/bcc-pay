using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using BccPay.Core.Cqrs;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Domain;
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
        private const string AllowOrigins = "_allowOrigins";

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var bccPaymentsConfiguration = Configuration.GetSection("BccPaymentsConfiguration");
            services.Configure<BccPaymentsConfiguration>(bccPaymentsConfiguration);

            services.AddRavenDatabaseDocumentStore();

            services.ConfigureBccPayInfrastructureServices(options =>
            {
                options.Nets.BaseAddress = "https://test.api.dibspayment.eu";
                options.Nets.CheckoutPageUrl = "http://localhost:8000";
                options.Nets.TermsUrl = "http://localhost:8000";
                options.Nets.SecretKey = Configuration["SecretKey"];
                options.Nets.NotificationUrl = "https://localhost:5001/Payment/webhook";
                options.Fixer.BaseAddress = "http://data.fixer.io/api";
            });

            services.ConfigureBccCoreCqrs();

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowOrigins,
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
                .AddJsonOptions(op => {
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

            app.UseCors(AllowOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.WarmUpIndexesInRavenDatabase();

            app.InitPaymentsConfiguration();
        }
    }
}
