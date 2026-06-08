using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class SetNotificationStatusCommandHandler : IRequestHandler<SetNotificationStatusCommand>
    {
        private readonly LearnerNotificationsDataContext _context;

        public SetNotificationStatusCommandHandler(LearnerNotificationsDataContext context)
        {
            _context = context;
        }
        
        public async Task Handle(SetNotificationStatusCommand request, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.LearnerAccountId == request.AccountIdentifier && 
                                          x.NotificationId == request.NotificationIdentifier, cancellationToken);

            if (notification == null)
            {
                return;
            }

            notification.StatusId = (byte)request.StatusId;

            var statusHistory = new StatusHistory
            {
                NotificationId = request.NotificationIdentifier,
                Status = (byte)request.StatusId,
                ChangeDate = DateTime.UtcNow
            };

            await _context.StatusHistory.AddAsync(statusHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
