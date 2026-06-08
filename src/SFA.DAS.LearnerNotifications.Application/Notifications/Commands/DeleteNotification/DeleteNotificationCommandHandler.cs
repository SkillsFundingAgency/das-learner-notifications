using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand>
    {
        private readonly LearnerNotificationsDataContext _context;

        public DeleteNotificationCommandHandler(LearnerNotificationsDataContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == request.NotificationIdentifier 
                                          && n.LearnerAccountId == request.AccountIdentifier, 
                    cancellationToken);

            if (notification == null)
            {
                return;
            }

            var statusHistoryEntries = await _context.StatusHistory
                .Where(sh => sh.NotificationId == request.NotificationIdentifier)
                .ToListAsync(cancellationToken);

            if (statusHistoryEntries.Any())
            {
                _context.StatusHistory.RemoveRange(statusHistoryEntries);
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
