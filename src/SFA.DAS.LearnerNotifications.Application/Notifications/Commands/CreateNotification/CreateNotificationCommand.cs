using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class CreateNotificationCommand
    {
        public Guid CorrelationId { get; set; }
        public Guid LearnerAccountId { get; set; }
        [RegularExpression(@"^[^<>]*$", ErrorMessage = "Category must not contain '<' or '>'.")]
        public string Category { get; set; } = null!;

        [RegularExpression(@"^[^<>]*$", ErrorMessage = "Heading must not contain '<' or '>'.")]
        public string Heading { get; set; } = null!;

        [RegularExpression(@"^[^<>]*$", ErrorMessage = "Body must not contain '<' or '>'.")]
        public string Body { get; set; } = null!;
        public byte StatusId { get; set; }
        public DateTime NotificationTime { get; set; }
        public DateTime TimeToExpire { get; set; }
        public DateTime TimeReceived { get; set; }
        public string? Link { get; set; }
        public byte Urgency { get; set; }
    }
}
