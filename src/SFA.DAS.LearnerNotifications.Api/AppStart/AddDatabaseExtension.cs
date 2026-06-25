extern alias AzureIdentityAlias;

using System;
using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using AzureIdentityAlias::Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LearnerNotifications.Data;

namespace SFA.DAS.LearnerNotifications.Api.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddDatabaseExtension
    {
        public static void AddDatabaseRegistration(this IServiceCollection services, IConfiguration config, string environmentName)
        {
            if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<LearnerNotificationsDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.LearnerNotifications"), ServiceLifetime.Transient);
            }
            else if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<LearnerNotificationsDataContext>(options => options.UseSqlServer(config["ApplicationSettings:SqlConnectionString"]), ServiceLifetime.Transient);
            }
            else
            {
                // Use the aliased version of DefaultAzureCredential
                services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
                services.AddDbContext<LearnerNotificationsDataContext>(ServiceLifetime.Transient);
            }
            
            services.AddTransient<ILearnerNotificationsDataContext, LearnerNotificationsDataContext>(provider => provider.GetService<LearnerNotificationsDataContext>());
            services.AddTransient(provider => new Lazy<LearnerNotificationsDataContext>(provider.GetService<LearnerNotificationsDataContext>()));
        }
    }
}