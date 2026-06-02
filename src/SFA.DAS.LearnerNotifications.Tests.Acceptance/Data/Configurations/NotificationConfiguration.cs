using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerNotifications.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Tests.Acceptance.Data.Configurations
{
    public class NotificationConfiguration: IEntityTypeConfiguration<SFA.DAS.LearnerNotifications.Models.Notification>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<SFA.DAS.LearnerNotifications.Models.Notification> builder)
        {
            builder.ToTable("Notification", "dbo");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).HasColumnName(@"Id").ValueGeneratedOnAdd();
            builder.Property(x => x.CorrelationId).HasColumnName(@"CorrelationId").IsRequired();
            builder.Property(x => x.LearnerAccountId).HasColumnName(@"LearnerAccountId").IsRequired();
            builder.Property(x => x.Category).HasColumnName(@"Category").IsRequired();
            builder.Property(x => x.Heading).HasColumnName(@"Heading").IsRequired();
            builder.Property(x => x.Body).HasColumnName(@"Body").IsRequired();
            builder.Property(x => x.LinkUrl).HasColumnName(@"LinkUrl").IsRequired(false);
            builder.Property(x => x.Status).HasColumnName(@"StatusId").IsRequired();
            builder.Property(x => x.NotificationTime).HasColumnName(@"NotificationTime").IsRequired(false);
            builder.Property(x => x.TimeToExpire).HasColumnName(@"TimeToExpire").IsRequired(false);
            builder.Property(x => x.TimeReceived).HasColumnName(@"TimeReceived").IsRequired(false);
        }        
    }
}
