namespace SFA.DAS.LearnerNotifications.Api.UnitTests.AppStart
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NUnit.Framework;
    using SFA.DAS.LearnerNotifications.Api.AppStart;

    [TestFixture]
    public static class ExceptionMiddlewareExtensionsTests
    {
        [Test]
        public static void CannotCallConfigureExceptionHandlerWithNullApp()
        {
            Assert.Throws<ArgumentNullException>(() => default(IApplicationBuilder).ConfigureExceptionHandler(Substitute.For<ILogger>()));
        }
    }
}