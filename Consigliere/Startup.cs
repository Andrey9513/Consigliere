using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Consigliere.Extensions;
using Consigliere.Features.Weather;
using Consigliere.HealthCheck;
using Consigliere.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Consigliere
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
            services.AddServiceHttpClients(Configuration);
            services.Configure<BotClientsOptions>(Configuration.GetSection(nameof(BotClientsOptions)));
            services.Configure<TelegramOptions>(Configuration.GetSection(nameof(TelegramOptions)));
            services.Configure<WeatherOptions>(Configuration.GetSection(nameof(WeatherOptions)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Consigliere",
                    Version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString(),
                    Description = "Consigliere Telegram bot API",
                });
                c.EnableAnnotations();
            });

            services.AddHealthChecks()
                .AddCheck<BaseHealthCheck>
                (
                    "base_health_check",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "base" }
                );

            services.AddControllers().AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddSingleton<WeatherAPI>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(swagOpt =>
            {
                swagOpt.SwaggerEndpoint("/swagger/v1/swagger.json", "Consigliere Telegram bot API");
                swagOpt.DefaultModelsExpandDepth(-1);
                swagOpt.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
           
        }
    }
}
