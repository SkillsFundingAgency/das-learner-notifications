using Microsoft.Extensions.Configuration;
using Reqnroll;
using Reqnroll.Assist;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Tests.Acceptance.StepDefinitions
{

    [Binding]
    public class TestRunBindings
    {
        public static IConfiguration Config { get; private set; }


        [BeforeTestRun]
        public static async Task SetUpMessaging()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appSettings.json"))
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appSettings.development.json"), true)
                .Build();
        }
    }
}
