using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Notifications;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Entities;
using SFA.DAS.LearnerNotifications.LearnerNotificationService.Functions;
using SFA.DAS.LearnerNotifications.Messages.Commands;
using UrgencyEnum = SFA.DAS.LearnerNotifications.Messages.Commands.Urgency;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.UnitTests.Steps;

[Binding]
public class SendNotificationSteps
{
    private readonly ScenarioContext _scenarioContext;
    private LearnerNotificationsDataContext _dbContext;
    private HandleSendNotification _function;
    private SendNotification _lastSentMessage;
    private Exception _lastException;

    public SendNotificationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<LearnerNotificationsDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new LearnerNotificationsDataContext(options);
        _dbContext.Database.EnsureCreated();

        if (!_dbContext.Statuses.Any())
        {
            _dbContext.Statuses.AddRange(
                new Status { Id = 1, Description = "Unread" },
                new Status { Id = 2, Description = "Acknowledged" },
                new Status { Id = 3, Description = "Hidden" },
                new Status { Id = 4, Description = "Expired" }
            );
        }
        if (!_dbContext.Urgencies.Any())
        {
            _dbContext.Urgencies.AddRange(
                new Domain.Entities.Urgency { Id = 1, Description = "Low" },
                new Domain.Entities.Urgency { Id = 2, Description = "Medium" },
                new Domain.Entities.Urgency { Id = 3, Description = "High" }
            );
        }
        _dbContext.SaveChanges();

        var notificationService = new NotificationService(_dbContext);
        var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<HandleSendNotification>>().Object;
        _function = new HandleSendNotification(notificationService, logger);
    }

    [AfterScenario]
    public async Task Cleanup()
    {
        await _dbContext.DisposeAsync();
    }

    [Given(@"the database is clean")]
    public void GivenTheDatabaseIsClean() { }

    [When(@"a SendNotification message is sent with:")]
    public async Task WhenASendNotificationMessageIsSentWith(Table table)
    {
        var row = table.Rows[0];
        var heading = row["Heading"];
        var body = row["Body"];
        var linkUrl = row.ContainsKey("LinkUrl") ? row["LinkUrl"] : null;
        var urgencyStr = row["Urgency"];
        var urgency = urgencyStr?.ToLower() switch
        {
            "low" => UrgencyEnum.Low,
            "medium" => UrgencyEnum.Medium,
            "high" => UrgencyEnum.High,
            _ => UrgencyEnum.Medium
        };

        var message = new SendNotification
        {
            CorrelationId = Guid.NewGuid(),
            LearnerAccountId = Guid.NewGuid(),
            Category = "TestCategory",
            Heading = heading,
            Body = body,
            LinkUrl = string.IsNullOrEmpty(linkUrl) ? null : linkUrl,
            NotificationTime = DateTime.UtcNow,
            TimeToExpire = DateTime.UtcNow.AddMonths(3),
            Urgency = urgency
        };
        _lastSentMessage = message;

        try
        {
            await _function.Run(message, CancellationToken.None);
            _lastException = null;
        }
        catch (Exception ex)
        {
            _lastException = ex;
        }
    }

    [When(@"a SendNotification message is sent with missing Heading")]
    public async Task WhenASendNotificationMessageIsSentWithMissingHeading()
    {
        var message = new SendNotification
        {
            CorrelationId = Guid.NewGuid(),
            LearnerAccountId = Guid.NewGuid(),
            Heading = null,
            Body = "Body",
            Urgency = UrgencyEnum.Low
        };
        _lastSentMessage = message;

        try
        {
            await _function.Run(message, CancellationToken.None);
            _lastException = null;
        }
        catch (Exception ex)
        {
            _lastException = ex;
        }
    }

    [Then(@"the notification should be stored in the database")]
    public void ThenTheNotificationShouldBeStoredInTheDatabase()
    {
        var stored = _dbContext.Notifications.FirstOrDefault(n => n.CorrelationId == _lastSentMessage.CorrelationId);
        stored.Should().NotBeNull();
        _scenarioContext.Set(stored, "StoredNotification");
    }

    [Then(@"the stored notification should have:")]
    public void ThenTheStoredNotificationShouldHave(Table table)
    {
        var row = table.Rows[0];
        var expectedHeading = row["Heading"];
        var expectedBody = row["Body"];
        var expectedLinkUrl = row.ContainsKey("LinkUrl") ? row["LinkUrl"] : null;
        var expectedStatusId = byte.Parse(row["StatusId"]);

        var notification = _scenarioContext.Get<Notification>("StoredNotification");
        notification.Heading.Should().Be(expectedHeading);
        notification.Body.Should().Be(expectedBody);
        if (expectedLinkUrl == null)
            notification.Link.Should().BeNull();
        else
            notification.Link.Should().Be(expectedLinkUrl);
        notification.StatusId.Should().Be(expectedStatusId);
    }

    [Then(@"the notification should have a valid CorrelationId and LearnerAccountId")]
    public void ThenTheNotificationShouldHaveValidIds()
    {
        var notification = _scenarioContext.Get<Notification>("StoredNotification");
        notification.CorrelationId.Should().NotBeNull();
        notification.CorrelationId.Value.Should().NotBe(Guid.Empty);
        notification.LearnerAccountId.Should().NotBeNull();
        notification.LearnerAccountId.Value.Should().NotBe(Guid.Empty);
    }

    [Then(@"the stored notification should have Link = NULL")]
    public void ThenTheStoredNotificationShouldHaveLinkNull()
    {
        var notification = _scenarioContext.Get<Notification>("StoredNotification");
        notification.Link.Should().BeNull();
    }

    [Then(@"the stored notification should have UrgencyId = (.*)")]
    public void ThenTheStoredNotificationShouldHaveUrgencyId(int expectedUrgencyId)
    {
        var notification = _scenarioContext.Get<Notification>("StoredNotification");
        notification.UrgencyId.Should().Be((byte)expectedUrgencyId);
    }

    [Then(@"the stored notification should have TimeReceived within the last (.*) seconds")]
    public void ThenTheStoredNotificationShouldHaveTimeReceivedWithinLastSeconds(int seconds)
    {
        var notification = _scenarioContext.Get<Notification>("StoredNotification");
        var now = DateTime.UtcNow;
        notification.TimeReceived.Should().NotBeNull();
        notification.TimeReceived.Value.Should().BeCloseTo(now, TimeSpan.FromSeconds(seconds));
    }

    [Then(@"no notification should be stored")]
    public void ThenNoNotificationShouldBeStored()
    {
        var count = _dbContext.Notifications.Count();
        count.Should().Be(0);
    }

    [Then(@"an error should be logged")]
    public void ThenAnErrorShouldBeLogged()
    {
        _lastException.Should().NotBeNull();
    }
}
