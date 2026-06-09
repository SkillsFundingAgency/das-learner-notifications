using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Data;
using Status = SFA.DAS.LearnerNotifications.Application.Models.Status;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationStatusQueryHandler : IRequestHandler<GetNotificationStatusQuery, GetNotificationStatusResult>
    {
        private readonly LearnerNotificationsDataContext _context;

        public GetNotificationStatusQueryHandler(LearnerNotificationsDataContext context)
        {
            _context = context;
        }
        
        public async Task<GetNotificationStatusResult> Handle(GetNotificationStatusQuery request, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .Where(x => x.LearnerAccountId == request.AccountIdentifier && 
                           x.NotificationId == request.NotificationIdentifier)
                .AsNoTracking()
                .Select(x => new { x.StatusId })
                .FirstOrDefaultAsync(cancellationToken);

            if (notification == null)
                return null;

            var latestHistory = await _context.StatusHistory
                .Where(x => x.NotificationId == request.NotificationIdentifier)
                .OrderByDescending(x => x.ChangeDate)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            byte statusId = notification.StatusId ?? 0; 
            
            return new GetNotificationStatusResult
            {
                StatusId = statusId,
                StatusName = GetStatusName(statusId),
                LastUpdated = latestHistory?.ChangeDate ?? DateTime.UtcNow
            };
        }

        private string GetStatusName(byte statusId)
        {
            if (Enum.IsDefined(typeof(Status), (int)statusId))
                return ((Status)statusId).ToString();
            
            return Status.Unread.ToString();
        }
    }
}
