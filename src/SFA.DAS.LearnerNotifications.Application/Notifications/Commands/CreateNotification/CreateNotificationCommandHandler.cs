using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Data;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand>
    {
        private readonly LearnerNotificationsDataContext _context;

        public CreateNotificationCommandHandler(LearnerNotificationsDataContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Domain.Entities.Notification
            {
                CorrelationId = request.CorrelationId,
                LearnerAccountId = request.LearnerAccountId,
                Category = request.Category,
                Heading = request.Heading,
                Body = request.Body,
                StatusId = request.StatusId,
                NotificationTime = request.NotificationTime,
                TimeToExpire = request.TimeToExpire,
                TimeReceived = request.TimeReceived,
                Link = request.Link,
                UrgencyId = request.Urgency
            };

            await _context.Notifications.AddAsync(notification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
