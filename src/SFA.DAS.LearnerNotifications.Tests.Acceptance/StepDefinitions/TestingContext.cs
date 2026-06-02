using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Tests.Acceptance.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Tests.Acceptance.StepDefinitions
{
    public class TestingContext
    {
        private IMessageSession messageSession;
        public TimeSpan TimeToWait { get; }
        public TimeSpan TimeToPause { get; }
        public IConfiguration Config { get; }
        public TestSessionDataContext DataContext { get; }

        private readonly Random random = new(Guid.NewGuid().GetHashCode());

        public TestingContext(IMessageSession messageSession, TestSessionDataContext dataContext)
        {
            this.messageSession = messageSession;
            Config = TestRunBindings.Config;
            DataContext = dataContext;
            TimeToWait = TimeSpan.Parse(Config["TimeToWait"] ?? "00:01:00");
            TimeToPause = TimeSpan.Parse(Config["PauseTime"] ?? "00:00:10");
        }

        //public async Task Send<T>(string messageJson)
        //{
        //    var message = System.Text.Json.JsonSerializer.Deserialize<T>(messageJson);
        //    await endpointInstance.Send("sfa-das-payments-collectionperiod", message);
        //}

        public async Task Send<T>(T message)
        {
            await messageSession.Send("sfa-das-learnernotifications", message);
        }

        public long GenerateId(int maxValue = 1000000)
        {
            var id = random.Next(maxValue);
            //TODO: make sure that the id isn't already in use.
            return id;
        }
        public async Task WaitForIt(Func<Task<bool>> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(TimeToWait);
            var lastRun = false;

            while (DateTime.Now < endTime || lastRun)
            {
                if (await lookForIt())
                {
                    if (lastRun) return;
                    lastRun = true;
                }
                else
                {
                    if (lastRun) break;
                }

                await Task.Delay(TimeToPause);
            }
            Assert.Fail($"{failText}  Time: {DateTime.Now:G}.");
        }

        public async Task WaitForIt(Func<bool> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(TimeToWait);
            var lastRun = false;

            while (DateTime.Now < endTime || lastRun)
            {
                if (lookForIt())
                {
                    if (lastRun) return;
                    lastRun = true;
                }
                else
                {
                    if (lastRun) break;
                }

                await Task.Delay(TimeToPause);
            }
            Assert.Fail($"{failText}  Time: {DateTime.Now:G}.");
        }
    }
}
