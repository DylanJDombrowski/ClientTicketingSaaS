using ClientTicketingSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientTicketingSaaS.Infrastructure.Data.Configurations;

public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.Property(tc => tc.Comment)
            .HasMaxLength(2000)
            .IsRequired();

        // Index for faster queries
        builder.HasIndex(tc => new { tc.TenantId, tc.TicketId });
    }
}