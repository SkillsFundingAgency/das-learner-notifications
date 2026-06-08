using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Application.Data
{
    public interface ILearnerNotificationsDataContext
    {
        DbSet<Models.Notification> Notifications { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class LearnerNotificationsDataContext : DbContext, ILearnerNotificationsDataContext
    {
        private readonly string connectionString;

        public DbSet<Models.Notification> Notifications { get; set; }

        public LearnerNotificationsDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public LearnerNotificationsDataContext(DbContextOptions<LearnerNotificationsDataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnerNotificationsDataContext).Assembly);
        }
    }
}
