using ClientTicketingSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientTicketingSaaS.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Company)
            .HasMaxLength(200);

        // Index for faster queries within tenant
        builder.HasIndex(c => new { c.TenantId, c.Email })
            .IsUnique();

        // Relationships
        builder.HasMany(c => c.Tickets)
            .WithOne(t => t.Client)
            .HasForeignKey(t => t.ClientId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete client if tickets exist
    }
}