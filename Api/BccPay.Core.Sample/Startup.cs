using BccPay.Core.Cqrs;
using BccPay.Core.Infrastructure.Validation;
using BccPay.Core.Sample.Mappers;
using BccPay.Core.Sample.Middleware;
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRavenDatabaseDocumentStore();

            services.ConfigureBccPayInfrastructure(options =>
            {
                options.Nets.BaseAddress = "https://test.api.dibspayment.eu";
                options.Nets.CheckoutPageUrl = "https://localhost:4000/";
                options.Nets.TermsUrl = "https://localhost:4000/";
                options.Nets.SecretKey = Configuration["SecretKey"];
            });

            services.ConfigureBccCoreCqrs();


            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.WarmUpIndexesInRavenDatabase();
        }
    }
}
