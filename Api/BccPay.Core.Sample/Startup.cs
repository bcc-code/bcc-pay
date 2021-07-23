using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Infrastructure.Extensions;
using BccPay.Core.Infrastructure.Payments.Extensions;
using BccPay.Core.Sample.Controllers;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using static BccPay.Core.Cqrs.Commands.CreatePaymentCommand;

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

            services.AddRefitClients();
            services.ConfigureBccPayInfrastructure();

            // TODO: move to service installer
            services.AddMediatR(typeof(BaseController).Assembly, typeof(CreatePaymentCommand).Assembly);
            services.AddValidation(new[] { typeof(CreatePaymentCommandValidator).Assembly });
            services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(new[] { typeof(CreatePaymentCommandValidator).Assembly }));

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
