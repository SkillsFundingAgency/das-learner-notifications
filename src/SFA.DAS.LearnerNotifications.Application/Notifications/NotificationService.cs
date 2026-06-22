using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Models;
using SFA.DAS.LearnerNotifications.Application.Queries;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly LearnerNotificationsDataContext _context;

        public NotificationService(LearnerNotificationsDataContext context)
        {
            _context = context;
        }

        public async Task<GetNotificationsByAccountIdentifierResult> GetNotificationsByAccountAsync(
            GetNotificationsByAccountIdentifierQuery query,
            CancellationToken cancellationToken)
        {
            var dbQuery = _context.Notifications
                .Where(x => x.LearnerAccountId == query.AccountIdentifier)
                .AsNoTracking();

            if (query.DateFrom.HasValue)
                dbQuery = dbQuery.Where(x => x.NotificationTime >= query.DateFrom.Value);

            if (query.Statuses != null && query.Statuses.Any())
            {
                var statusIds = query.Statuses.Select(s => (byte?)s).ToList();
                dbQuery = dbQuery.Where(x => statusIds.Contains(x.StatusId));
            }

            dbQuery = query.Order == SortOrder.Ascending
                ? dbQuery.OrderBy(x => x.NotificationTime)
                : dbQuery.OrderByDescending(x => x.NotificationTime);

            var notifications = await dbQuery.ToListAsync(cancellationToken);
            return new GetNotificationsByAccountIdentifierResult { Notifications = notifications };
        }

        public async Task<Notification?> GetNotificationByIdAsync(
            GetNotificationByIdQuery query,
            CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .Where(x => x.LearnerAccountId == query.AccountIdentifier &&
                            x.NotificationId == query.NotificationIdentifier)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<GetNotificationStatusResult?> GetNotificationStatusAsync(
            GetNotificationStatusQuery query,
            CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .Where(x => x.LearnerAccountId == query.AccountIdentifier &&
                            x.NotificationId == query.NotificationIdentifier)
                .AsNoTracking()
                .Select(x => new { x.StatusId })
                .FirstOrDefaultAsync(cancellationToken);

            if (notification == null)
                return null;

            var latestHistory = await _context.StatusHistory
                .Where(x => x.NotificationId == query.NotificationIdentifier)
                .OrderByDescending(x => x.ChangeDate)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            byte statusId = notification.StatusId ?? 0;
            
            // Fully qualify the Status enum to avoid ambiguity with Domain.Entities.Status
            string GetStatusName(byte id) =>
                Enum.IsDefined(typeof(Application.Models.Status), (int)id) 
                    ? ((Application.Models.Status)id).ToString() 
                    : Application.Models.Status.Unread.ToString();

            return new GetNotificationStatusResult
            {
                StatusId = statusId,
                StatusName = GetStatusName(statusId),
                LastUpdated = latestHistory?.ChangeDate ?? DateTime.UtcNow
            };
        }

        public async Task SetNotificationStatusAsync(
            SetNotificationStatusCommand command,
            CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.LearnerAccountId == command.AccountIdentifier &&
                                          x.NotificationId == command.NotificationIdentifier,
                    cancellationToken);

            if (notification == null) return;

            notification.StatusId = (byte)command.StatusId;

            var statusHistory = new StatusHistory
            {
                NotificationId = command.NotificationIdentifier,
                Status = (byte)command.StatusId,
                ChangeDate = DateTime.UtcNow
            };

            await _context.StatusHistory.AddAsync(statusHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateNotificationAsync(
            CreateNotificationCommand command,
            CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                CorrelationId = command.CorrelationId,
                LearnerAccountId = command.LearnerAccountId,
                Category = command.Category,
                Heading = command.Heading,
                Body = command.Body,
                StatusId = command.StatusId,
                NotificationTime = command.NotificationTime,
                TimeToExpire = command.TimeToExpire,
                TimeReceived = command.TimeReceived,
                Link = command.Link,
                UrgencyId = command.Urgency
            };

            await _context.Notifications.AddAsync(notification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteNotificationAsync(
            DeleteNotificationCommand command,
            CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == command.NotificationIdentifier &&
                                          n.LearnerAccountId == command.AccountIdentifier,
                    cancellationToken);

            if (notification == null) return;

            var statusHistoryEntries = await _context.StatusHistory
                .Where(sh => sh.NotificationId == command.NotificationIdentifier)
                .ToListAsync(cancellationToken);

            if (statusHistoryEntries.Any())
                _context.StatusHistory.RemoveRange(statusHistoryEntries);

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        public async Task<int> DeleteExpiredNotificationsAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var expiredNotifications = await _context.Notifications
                .Where(n => n.TimeToExpire < now)
                .ToListAsync(cancellationToken);

            if (!expiredNotifications.Any())
                return 0;

            var notificationIds = expiredNotifications.Select(n => n.NotificationId).ToList();

            // Delete associated status history
            var statusHistories = await _context.StatusHistory
                .Where(sh => sh.NotificationId.HasValue && notificationIds.Contains(sh.NotificationId.Value))
                .ToListAsync(cancellationToken);

            if (statusHistories.Any())
                _context.StatusHistory.RemoveRange(statusHistories);

            _context.Notifications.RemoveRange(expiredNotifications);
            await _context.SaveChangesAsync(cancellationToken);

            return expiredNotifications.Count;
        }
    }
}
