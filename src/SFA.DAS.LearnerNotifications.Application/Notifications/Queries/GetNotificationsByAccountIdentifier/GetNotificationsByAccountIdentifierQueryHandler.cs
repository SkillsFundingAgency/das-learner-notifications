using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Application.Models;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Data;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationsByAccountIdentifierQueryHandler : IRequestHandler<GetNotificationsByAccountIdentifierQuery, GetNotificationsByAccountIdentifierResult>
    {
        private readonly LearnerNotificationsDataContext _context;

        public GetNotificationsByAccountIdentifierQueryHandler(LearnerNotificationsDataContext context)
        {
            _context = context;
        }

        public async Task<GetNotificationsByAccountIdentifierResult> Handle(GetNotificationsByAccountIdentifierQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Notifications
                .Where(x => x.LearnerAccountId == request.AccountIdentifier)
                .AsNoTracking();

            if (request.DateFrom.HasValue)
            {
                query = query.Where(x => x.NotificationTime >= request.DateFrom.Value);
            }

            if (request.Statuses != null && request.Statuses.Any())
            {
                var statusIds = request.Statuses.Select(s => (byte?)s).ToList();
                query = query.Where(x => statusIds.Contains(x.StatusId));
            }

            query = request.Order == SortOrder.Ascending
                ? query.OrderBy(x => x.NotificationTime)
                : query.OrderByDescending(x => x.NotificationTime);

            var notifications = await query.ToListAsync(cancellationToken);
            return new GetNotificationsByAccountIdentifierResult { Notifications = notifications };
        }
    }
}
