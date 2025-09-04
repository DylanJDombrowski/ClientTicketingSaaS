using ClientTicketingSaaS.Domain.Entities;
using ClientTicketingSaaS.Infrastructure.Identity; // Add this using statement!
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientTicketingSaaS.Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(4000)
            .IsRequired();

        // Index for faster queries
        builder.HasIndex(t => new { t.TenantId, t.Status });
        builder.HasIndex(t => new { t.TenantId, t.ClientId });

        // Relationships
        // This configures the relationship between Ticket and ApplicationUser
        // Even though Domain doesn't have the navigation property
        builder.HasOne<ApplicationUser>() // No navigation property in Domain
            .WithMany() // No back-reference needed
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Comments)
            .WithOne(c => c.Ticket)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TimeEntries)
            .WithOne(te => te.Ticket)
            .HasForeignKey(te => te.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Convert enums to integers in database
        builder.Property(t => t.Status)
            .HasConversion<int>();

        builder.Property(t => t.Priority)
            .HasConversion<int>();
    }
}