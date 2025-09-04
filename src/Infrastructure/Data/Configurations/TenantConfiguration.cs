using ClientTicketingSaaS.Domain.Entities;
using ClientTicketingSaaS.Infrastructure.Identity; // Add this using statement!
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientTicketingSaaS.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Subdomain)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(t => t.Subdomain)
            .IsUnique();

        builder.Property(t => t.CustomDomain)
            .HasMaxLength(200);

        // Convert enum to integer in database
        builder.Property(t => t.Plan)
            .HasConversion<int>();

        // Relationships - Infrastructure can configure relationships between Domain and Infrastructure!
        builder.HasMany<ApplicationUser>() // No navigation property in Domain, but we can still configure the relationship
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Clients)
            .WithOne(c => c.Tenant)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Tickets)
            .WithOne(t => t.Tenant)
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}