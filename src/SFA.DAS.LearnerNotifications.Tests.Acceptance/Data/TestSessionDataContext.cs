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

        public TestSessionDataContext(DbContextOptions<TestSessionDataContext> options) : base(options)
        {
        }
            
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
        }
    }
}
