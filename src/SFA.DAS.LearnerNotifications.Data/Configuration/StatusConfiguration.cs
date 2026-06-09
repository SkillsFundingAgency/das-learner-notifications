using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable("Status");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .HasColumnType("tinyint")
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasColumnType("nvarchar")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasIndex(x => x.Description);
        }
    }
}
