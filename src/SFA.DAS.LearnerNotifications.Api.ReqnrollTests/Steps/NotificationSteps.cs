using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Reqnroll;
using SFA.DAS.LearnerNotifications.Api;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Api.ReqnrollTests.Steps;

[Binding]
public class NotificationSteps
{
    private readonly ScenarioContext _scenarioContext;
    private WebApplicationFactory<Startup> _factory = null!;
    private HttpClient _client = null!;
    private HttpResponseMessage _lastResponse = null!;
    private Guid _learnerAccountId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    public NotificationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["EnvironmentName"] = "DEV",
                        ["AzureAd:tenant"] = "test-tenant",
                        ["AzureAd:identifier"] = "https://test-identifier",
                        ["LearnerNotifications:SqlConnectionString"] = "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;MultipleActiveResultSets=true"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(LearnerNotificationsDataContext));
                    if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

                    var optionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LearnerNotificationsDataContext>));
                    if (optionsDescriptor != null) services.Remove(optionsDescriptor);

                    services.AddDbContext<LearnerNotificationsDataContext>(options =>
                        options.UseInMemoryDatabase("TestDb_Fixed"));
                });
            });
        _client = _factory.CreateClient();
        
        // Add required API version header
        _client.DefaultRequestHeaders.Add("X-Version", "1.0");

        // Reset database before each scenario
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    [AfterScenario]
    public async Task TearDown()
    {
        _client?.Dispose();
        if (_factory != null)
            await _factory.DisposeAsync();
    }

    [Given(@"a learner account with id ""([^""]*)""")]
    public void GivenALearnerAccountWithId(string accountId)
    {
        _learnerAccountId = Guid.Parse(accountId);
        _scenarioContext["LearnerAccountId"] = _learnerAccountId;
    }

    [Given(@"no existing notifications")]
    public async Task GivenNoExistingNotifications()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        db.Notifications.RemoveRange(db.Notifications);
        await db.SaveChangesAsync();
    }

    [When(@"I create a notification with heading ""([^""]*)"" and body ""([^""]*)""")]
    public async Task WhenICreateANotification(string heading, string body)
    {
        var request = new
        {
            CorrelationId = Guid.NewGuid(),
            Category = "Test",
            Heading = heading,
            Body = body,
            StatusId = 1,
            NotificationTime = DateTime.UtcNow,
            TimeToExpire = DateTime.UtcNow.AddDays(7),
            TimeReceived = DateTime.UtcNow,
            Link = "http://test.com",
            Urgency = 1
        };
        _lastResponse = await _client.PostAsJsonAsync($"/learner/{_learnerAccountId}/notifications", request);
    }

    [Then(@"the response status should be (.*)")]
    public void ThenResponseStatusShouldBe(string statusText)
    {
        var match = Regex.Match(statusText, @"\d+");
        if (!match.Success)
            throw new FormatException($"Could not extract status code from: {statusText}");
        var expectedStatusCode = int.Parse(match.Value);
        _lastResponse.StatusCode.Should().Be((HttpStatusCode)expectedStatusCode);
    }

    [Then(@"the notification should exist for that learner")]
    public async Task ThenNotificationShouldExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        var exists = await db.Notifications.AnyAsync(n => n.LearnerAccountId == _learnerAccountId);
        exists.Should().BeTrue();
    }

    [Given(@"the learner has (.*) existing notifications?")]
    public async Task GivenTheLearnerHasExistingNotifications(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        for (int i = 1; i <= count; i++)
        {
            db.Notifications.Add(new Notification
            {
                NotificationId = i,
                LearnerAccountId = _learnerAccountId,
                Heading = $"Sample {i}",
                Body = "Body",
                StatusId = 1,
                NotificationTime = DateTime.UtcNow.AddDays(-i)
            });
        }
        await db.SaveChangesAsync();
    }

    [When(@"I request all notifications")]
    public async Task WhenIRequestAllNotifications()
    {
        _lastResponse = await _client.GetAsync($"/learner/{_learnerAccountId}");
    }

    [Then(@"the response should contain exactly (.*) notifications")]
    public async Task ThenResponseShouldContainExactlyNotifications(int expectedCount)
    {
        var result = await _lastResponse.Content.ReadFromJsonAsync<GetNotificationsByAccountIdentifierResult>();
        result.Should().NotBeNull();
        result!.Notifications.Count.Should().Be(expectedCount);
    }

    [Given(@"the learner has (.*) existing notification with id (.*)")]
    public async Task GivenLearnerHasNotificationWithId(int count, long id)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        db.Notifications.Add(new Notification
        {
            NotificationId = id,
            LearnerAccountId = _learnerAccountId,
            Heading = "Sample Heading",
            Body = "Body",
            StatusId = 1,
            NotificationTime = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    [When(@"I request notification with id (.*)")]
    public async Task WhenIRequestNotificationWithId(long id)
    {
        _lastResponse = await _client.GetAsync($"/learner/{_learnerAccountId}/notifications/{id}");
    }

    [Then(@"the notification heading should be ""([^""]*)""")]
    public async Task ThenNotificationHeadingShouldBe(string expectedHeading)
    {
        var notification = await _lastResponse.Content.ReadFromJsonAsync<Notification>();
        notification.Should().NotBeNull();
        notification!.Heading.Should().Be(expectedHeading);
    }

    [When(@"I request the status of notification (.*)")]
    public async Task WhenIRequestStatusOfNotification(long id)
    {
        _lastResponse = await _client.GetAsync($"/learner/{_learnerAccountId}/notifications/{id}/status");
    }

    [Then(@"the status id should be (.*) \(Unread\)")]
    public async Task ThenStatusIdShouldBeUnread(int expectedStatusId)
    {
        var status = await _lastResponse.Content.ReadFromJsonAsync<GetNotificationStatusResult>();
        status.Should().NotBeNull();
        status!.StatusId.Should().Be((byte)expectedStatusId);
    }

    [When(@"I set the status of notification (.*) to (.*) \(Acknowledged\)")]
    public async Task WhenISetNotificationStatus(long id, int newStatusId)
    {
        var payload = new { StatusId = newStatusId };
        _lastResponse = await _client.PutAsJsonAsync($"/learner/{_learnerAccountId}/notifications/{id}/status", payload);
    }

    [Then(@"the status of notification (.*) should be (.*)")]
    public async Task ThenNotificationStatusShouldBe(long id, int expectedStatusId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id && n.LearnerAccountId == _learnerAccountId);
        notification.Should().NotBeNull();
        notification!.StatusId.Should().Be((byte)expectedStatusId);
    }

    [When(@"I delete notification (.*)")]
    public async Task WhenIDeleteNotification(long id)
    {
        _lastResponse = await _client.DeleteAsync($"/learner/{_learnerAccountId}/notifications/{id}");
        if (_lastResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await _lastResponse.Content.ReadAsStringAsync();
            throw new Exception($"DELETE returned 400. Response: {error}");
        }
    }

    [Then(@"the notification (.*) should no longer exist")]
    public async Task ThenNotificationShouldNotExist(long id)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        var exists = await db.Notifications.AnyAsync(n => n.NotificationId == id && n.LearnerAccountId == _learnerAccountId);
        exists.Should().BeFalse();
    }

    [Given(@"the learner has notifications created on ""([^""]*)"" and ""([^""]*)""")]
    public async Task GivenNotificationsOnDates(string date1, string date2)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        db.Notifications.Add(new Notification
        {
            NotificationId = 100,
            LearnerAccountId = _learnerAccountId,
            Heading = "Old",
            NotificationTime = DateTime.Parse(date1),
            StatusId = 1
        });
        db.Notifications.Add(new Notification
        {
            NotificationId = 101,
            LearnerAccountId = _learnerAccountId,
            Heading = "New",
            NotificationTime = DateTime.Parse(date2),
            StatusId = 1
        });
        await db.SaveChangesAsync();
    }

    [When(@"I request notifications from date ""([^""]*)""")]
    public async Task WhenRequestFromDate(string fromDate)
    {
        _lastResponse = await _client.GetAsync($"/learner/{_learnerAccountId}?dateFrom={fromDate}");
    }

    [Then(@"only notifications after ""([^""]*)"" are returned")]
    public async Task ThenOnlyNotificationsAfterDate(string fromDate)
    {
        var result = await _lastResponse.Content.ReadFromJsonAsync<GetNotificationsByAccountIdentifierResult>();
        result.Should().NotBeNull();
        result!.Notifications.Should().NotBeEmpty("because at least one notification should match the date filter");
        result.Notifications.Should().AllSatisfy(n => n.NotificationTime.Should().BeOnOrAfter(DateTime.Parse(fromDate)));
    }

    [Given(@"the learner has notifications with status (.*) \(Unread\) and (.*) \(Acknowledged\)")]
    public async Task GivenNotificationsWithStatuses(int status1, int status2)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnerNotificationsDataContext>();
        db.Notifications.Add(new Notification { NotificationId = 200, LearnerAccountId = _learnerAccountId, Heading = "Unread", StatusId = (byte)status1 });
        db.Notifications.Add(new Notification { NotificationId = 201, LearnerAccountId = _learnerAccountId, Heading = "Acknowledged", StatusId = (byte)status2 });
        await db.SaveChangesAsync();
    }

    [When(@"I request notifications with status ""([^""]*)""")]
    public async Task WhenRequestWithStatus(string statusName)
    {
        var statusEnum = (Application.Models.Status)Enum.Parse(typeof(Application.Models.Status), statusName);
        int statusId = (int)statusEnum;
        _lastResponse = await _client.GetAsync($"/learner/{_learnerAccountId}?statuses={statusId}");
    }

    [Then(@"only notifications with status (.*) are returned")]
    public async Task ThenOnlyStatusReturned(int expectedStatusId)
    {
        var result = await _lastResponse.Content.ReadFromJsonAsync<GetNotificationsByAccountIdentifierResult>();
        result.Should().NotBeNull();
        result!.Notifications.Should().NotBeEmpty();
        result.Notifications.Should().AllSatisfy(n => n.StatusId.Should().Be((byte)expectedStatusId));
    }
}
