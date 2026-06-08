using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, Notification>
    {
        private readonly LearnerNotificationsDataContext _context;

        public GetNotificationByIdQueryHandler(LearnerNotificationsDataContext context)
        {
            _context = context;
        }
        
        public async Task<Notification> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .Where(x => x.LearnerAccountId == request.AccountIdentifier && 
                            x.NotificationId == request.NotificationIdentifier)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return notification;
        }
    }
}
