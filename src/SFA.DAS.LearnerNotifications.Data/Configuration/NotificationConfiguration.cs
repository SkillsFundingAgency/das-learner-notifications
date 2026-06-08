using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");
            builder.HasKey(x => x.NotificationId);
            
            builder.Property(x => x.NotificationId)
                .HasColumnName("NotificationId")
                .HasColumnType("bigint")
                .IsRequired()
                .ValueGeneratedOnAdd();
            
            builder.Property(x => x.CorrelationId).HasColumnName("CorrelationId").HasColumnType("uniqueidentifier");
            builder.Property(x => x.LearnerAccountId).HasColumnName("LearnerAccountId").HasColumnType("uniqueidentifier");
            builder.Property(x => x.Category).HasColumnName("Category").HasColumnType("nvarchar(255)").HasMaxLength(255);
            builder.Property(x => x.Heading).HasColumnName("Heading").HasColumnType("nvarchar(255)").HasMaxLength(255);
            builder.Property(x => x.Body).HasColumnName("Body").HasColumnType("nvarchar(max)");
            builder.Property(x => x.StatusId).HasColumnName("StatusId").HasColumnType("tinyint");
            builder.Property(x => x.NotificationTime).HasColumnName("NotificationTime").HasColumnType("datetime");
            builder.Property(x => x.TimeToExpire).HasColumnName("TimeToExpire").HasColumnType("datetime");
            builder.Property(x => x.TimeReceived).HasColumnName("TimeReceived").HasColumnType("datetime");
            
            builder.HasIndex(x => x.NotificationId).IsUnique();
            builder.HasIndex(x => x.CorrelationId);
            builder.HasIndex(x => x.LearnerAccountId);
            builder.HasIndex(x => x.StatusId);
            builder.HasIndex(x => x.NotificationTime);
        }
    }
}
