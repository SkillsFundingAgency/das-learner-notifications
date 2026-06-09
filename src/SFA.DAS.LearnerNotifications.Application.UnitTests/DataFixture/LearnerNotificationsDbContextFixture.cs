using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Data;

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
        }

        public LearnerNotificationsDataContext DbContext { get; private set; }

        [TearDown]
        public void TearDown() => DbContext?.Dispose();
    }
}