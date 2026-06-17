using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
    {
        public void Configure(EntityTypeBuilder<StatusHistory> builder)
        {
            builder.ToTable("StatusHistory");
            builder.HasKey(x => x.StatusHistoryId);
            
            builder.Property(x => x.StatusHistoryId).HasColumnName("StatusHistoryId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.NotificationId).HasColumnName("NotificationId").HasColumnType("bigint");
            builder.Property(x => x.Status).HasColumnName("Status").HasColumnType("tinyint");
            builder.Property(x => x.ChangeDate).HasColumnName("ChangeDate").HasColumnType("datetime");
            
            builder.HasIndex(x => x.StatusHistoryId).IsUnique();
            builder.HasIndex(x => x.NotificationId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.ChangeDate);
        }
    }
}
