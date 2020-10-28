using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PaymentGateway.Controllers;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Library.Services;
using PaymentGateway.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PaymentGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Payment Gateway Api", Version = "v1" });
            });

            services.AddDbContext<PaymentsContext>(options => options.UseInMemoryDatabase(databaseName: Configuration["DatabaseName"]),
                ServiceLifetime.Scoped,
                ServiceLifetime.Scoped);
            services.AddHttpClient<IBankService, BankService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["BankEndpoint"]);
                client.DefaultRequestHeaders.Add("Client Name", "PaymentGateway");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration["BankClient:AuthorisationKey"]);
            });
            services.AddControllers();
            services.AddScoped<PaymentController>();
            services.AddScoped<PaymentDetailsController>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddTransient<IDbRespository<PaymentDetails>, PaymentRepository>();
            services.AddTransient<IDbRespository<IdempotencyKey>, IdempotencyKeyRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
