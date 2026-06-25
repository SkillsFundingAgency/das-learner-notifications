Feature: Learner Notifications API
  As a learner
  I want to manage my notifications
  So that I can stay informed and take action

Background:
  Given a learner account with id "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  And no existing notifications

Scenario: Create a new notification
  When I create a notification with heading "Test Heading" and body "Test Body"
  Then the response status should be 200 OK
  And the notification should exist for that learner

Scenario: Get all notifications
  Given the learner has 2 existing notifications
  When I request all notifications
  Then the response status should be 200 OK
  And the response should contain exactly 2 notifications

Scenario: Get a single notification by id
  Given the learner has 1 existing notification with id 1000
  When I request notification with id 1000
  Then the response status should be 200 OK
  And the notification heading should be "Sample Heading"

Scenario: Get notification status
  Given the learner has 1 existing notification with id 1000
  When I request the status of notification 1000
  Then the response status should be 200 OK
  And the status id should be 1 (Unread)

Scenario: Update notification status
  Given the learner has 1 existing notification with id 1000
  When I set the status of notification 1000 to 2 (Acknowledged)
  Then the response status should be 200 OK
  And the status of notification 1000 should be 2

Scenario: Delete a notification
  Given the learner has 1 existing notification with id 1000
  When I delete notification 1000
  Then the response status should be 204 No Content
  And the notification 1000 should no longer exist

Scenario: Request notifications with date filter
  Given the learner has notifications created on "2025-01-10" and "2025-01-20"
  When I request notifications from date "2025-01-15"
  Then only notifications after "2025-01-15" are returned

Scenario: Request notifications with status filter
  Given the learner has notifications with status 1 (Unread) and 2 (Acknowledged)
  When I request notifications with status "Unread"
  Then only notifications with status 1 are returned