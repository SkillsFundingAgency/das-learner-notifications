using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.LearnerNotifications.Data.Configuration;
using SFA.DAS.LearnerNotifications.Domain.Configuration;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Data
{
    public interface ILearnerNotificationsDataContext
    {
        DbSet<Notification> Notifications { get; set; }
        DbSet<StatusHistory> StatusHistory { get; set; }
        DbSet<Status> Statuses { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    [ExcludeFromCodeCoverage]
    public partial class LearnerNotificationsDataContext : DbContext, ILearnerNotificationsDataContext
    {
        private const string AzureResource = "https://database.windows.net/";

        // New entities for Notifications feature
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<StatusHistory> StatusHistory { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }

        private readonly LearnerNotificationsConfiguration _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

        public LearnerNotificationsDataContext()
        {
        }

        public LearnerNotificationsDataContext(DbContextOptions options) : base(options)
        {

        }

        public LearnerNotificationsDataContext(IOptions<LearnerNotificationsConfiguration> config, DbContextOptions options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
        {
            _configuration = config.Value;
            _azureServiceTokenProvider = azureServiceTokenProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            if (_configuration == null || _azureServiceTokenProvider == null)
            {
                return;
            }

            var connection = new SqlConnection
            {
                ConnectionString = _configuration.SqlConnectionString,
                AccessToken = _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result,
            };

            optionsBuilder.UseSqlServer(connection, options =>
                options.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(20),
                    null
                ));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations for all entities
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new StatusHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new StatusConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
