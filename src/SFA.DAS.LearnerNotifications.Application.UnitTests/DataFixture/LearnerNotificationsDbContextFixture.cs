using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.UnitTests.DataFixture
{
    public class LearnerNotificationsDbContextFixture
    {
        [SetUp]
        public void BaseSetup()
        {
            var options = new DbContextOptionsBuilder<LearnerNotificationsDataContext>()
                .UseInMemoryDatabase($"SFA.DAS.LearnerNotifications.Database_{DateTime.UtcNow.ToFileTimeUtc()}")
                .EnableSensitiveDataLogging()
                .Options;

            DbContext = new LearnerNotificationsDataContext(options);

            if (!DbContext.Urgencies.Any())
            {
                DbContext.Urgencies.AddRange(
                    new Urgency { Id = 1, Description = "Low" },
                    new Urgency { Id = 2, Description = "Medium" },
                    new Urgency { Id = 3, Description = "High" }
                );
                DbContext.SaveChanges();
            }
        }

        public LearnerNotificationsDataContext DbContext { get; private set; }

        [TearDown]
        public void TearDown() => DbContext?.Dispose();
    }
}
