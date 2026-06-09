using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Application.Data.Configuration
{
    public class NotificationConfiguration: IEntityTypeConfiguration<Notification>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification", "dbo");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).HasColumnName(@"Id").ValueGeneratedOnAdd();
            builder.Property(x => x.CorrelationId).HasColumnName(@"CorrelationId").IsRequired();
            builder.Property(x => x.LearnerAccountId).HasColumnName(@"LearnerAccountId").IsRequired();
            builder.Property(x => x.Category).HasColumnName(@"Category").IsRequired(false);
            builder.Property(x => x.Heading).HasColumnName(@"Heading").IsRequired();
            builder.Property(x => x.Body).HasColumnName(@"Body").IsRequired();
            builder.Property(x => x.LinkUrl).HasColumnName(@"LinkUrl").IsRequired(false);
            builder.Property(x => x.Status).HasColumnName(@"Status").IsRequired();
            builder.Property(x => x.NotificationTime).HasColumnName(@"NotificationTime").IsRequired();
            builder.Property(x => x.TimeToExpire).HasColumnName(@"TimeToExpire");
            builder.Property(x => x.TimeReceived).HasColumnName(@"TimeReceived").IsRequired();
            builder.Property(x => x.Urgency).HasColumnName(@"Urgency");
        }        
    }
}
