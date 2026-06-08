namespace SFA.DAS.LearnerNotifications.Api.UnitTests.AppStart
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using NUnit.Framework;
    using SFA.DAS.LearnerNotifications.Api.AppStart;

    [TestFixture]
    public static class HealthCheckStartupTests
    {
        [Test]
        public static void CannotCallUseHealthChecksWithNullApp()
        {
            Assert.Throws<ArgumentNullException>(() => default(IApplicationBuilder).UseHealthChecks());
        }
    }
}