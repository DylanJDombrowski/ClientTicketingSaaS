using ClientTicketingSaaS.Domain.Entities;
using ClientTicketingSaaS.Infrastructure.Identity; // Add this using statement!
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientTicketingSaaS.Infrastructure.Data.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.Property(te => te.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(te => te.Hours)
            .HasColumnType("decimal(5,2)"); // Up to 999.99 hours

        // Index for faster queries
        builder.HasIndex(te => new { te.TenantId, te.TicketId });
        builder.HasIndex(te => new { te.TenantId, te.UserId });

        // Relationships - Configure the relationship with ApplicationUser
        builder.HasOne<ApplicationUser>() // No navigation property in Domain
            .WithMany(u => u.TimeEntries)
            .HasForeignKey(te => te.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}