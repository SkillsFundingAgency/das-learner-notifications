using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.LearnerNotifications.Api.AppStart;
using SFA.DAS.LearnerNotifications.Api.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.LearnerNotifications.Api.AppStart;

namespace SFA.DAS.LearnerNotifications.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {

#if DEBUG
                config
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Development.json", true);
#endif

                config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                }
                );
            }

            _configuration = config.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AzureActiveDirectoryConfiguration>(_configuration.GetSection("AzureAd"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<AzureActiveDirectoryConfiguration>>().Value);
            
            if (!ConfigurationIsLocalOrDev())
            {
                var azureAdConfiguration = _configuration
                    .GetSection("AzureAd")
                    .Get<AzureActiveDirectoryConfiguration>();

                var policies = new Dictionary<string, string>
                {
                    {PolicyNames.Default, RoleNames.Default}
                };

                services.AddAuthentication(azureAdConfiguration, policies);
            }
            
            services
                .AddMvc(o =>
                {
                    if (!ConfigurationIsLocalOrDev())
                    {
                        o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string> { PolicyNames.DataLoad }));
                    }
                    o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            
            services.AddOpenTelemetryRegistration(_configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LearnerNotificationsApi", Version = "v1" });
                c.SwaggerDoc("operations", new OpenApiInfo { Title = "LearnerNotificationsApi operations" });
                c.OperationFilter<SwaggerVersionHeaderFilter>();
            });

            services.AddApiVersioning(opt => {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LearnerNotificationsAPI v1");
                c.SwaggerEndpoint("/swagger/operations/swagger.json", "Operations v1");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler(logger);

            app.UseAuthentication();

            if (!_configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
            }

            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                builder.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Agencies}/{action=Index}/{id?}");
            });
        }

        private bool ConfigurationIsLocalOrDev()
        {
            return _configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   _configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
