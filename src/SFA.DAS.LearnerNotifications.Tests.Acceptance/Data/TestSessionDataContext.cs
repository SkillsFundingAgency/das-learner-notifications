using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Tests.Acceptance.Data
{
    public  class TestSessionDataContext: DbContext
    {
        private readonly string connectionString;

        public virtual DbSet<Models.Notification> Notifications { get; set; }

        public TestSessionDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

          
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TestSessionDataContext).Assembly);
        }
    }
}
