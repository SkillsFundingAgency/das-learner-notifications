Feature: Process Send Notifications
  As a learner notification system
  I want to receive SendNotification messages
  So that they are stored in the database and can be viewed later

Background:
  Given the database is clean

Scenario: Successfully stores a notification with all fields
  When a SendNotification message is sent with:
    | Heading          | Body                     | LinkUrl           | Urgency |
    | Test Heading     | Test body content        | http://test.com   | Medium  |
  Then the notification should be stored in the database
  And the stored notification should have:
    | Heading          | Body                     | LinkUrl           | StatusId |
    | Test Heading     | Test body content        | http://test.com   | 1        |
  And the notification should have a valid CorrelationId and LearnerAccountId

Scenario: Stores a notification with null LinkUrl
  When a SendNotification message is sent with:
    | Heading          | Body               | LinkUrl | Urgency |
    | No Link Heading  | Body without link  |         | Low     |
  Then the notification should be stored in the database
  And the stored notification should have Link = NULL

Scenario: Stores a notification with High urgency
  When a SendNotification message is sent with:
    | Heading       | Body          | LinkUrl | Urgency |
    | Urgent Alert  | Very urgent   | /alert  | High    |
  Then the notification should be stored in the database
  And the stored notification should have UrgencyId = 3

Scenario: Sets TimeReceived to current timestamp
  When a SendNotification message is sent with:
    | Heading        | Body       | LinkUrl | Urgency |
    | TimeCheck      | Check time | /time   | Medium  |
  Then the notification should be stored in the database
  And the stored notification should have TimeReceived within the last 5 seconds

Scenario: Does not store a notification if required fields are missing (heading/body validation)
  When a SendNotification message is sent with missing Heading
  Then no notification should be stored
  And an error should be logged