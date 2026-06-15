using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Queries;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.Notifications
{
    public interface INotificationService
    {
        Task<GetNotificationsByAccountIdentifierResult> GetNotificationsByAccountAsync(
            GetNotificationsByAccountIdentifierQuery query,
            CancellationToken cancellationToken);
        
        Task<Notification?> GetNotificationByIdAsync(
            GetNotificationByIdQuery query,
            CancellationToken cancellationToken);
        
        Task<GetNotificationStatusResult?> GetNotificationStatusAsync(
            GetNotificationStatusQuery query,
            CancellationToken cancellationToken);
        
        Task SetNotificationStatusAsync(
            SetNotificationStatusCommand command,
            CancellationToken cancellationToken);
        
        Task CreateNotificationAsync(
            CreateNotificationCommand command,
            CancellationToken cancellationToken);
        
        Task DeleteNotificationAsync(
            DeleteNotificationCommand command,
            CancellationToken cancellationToken);
    }
}
