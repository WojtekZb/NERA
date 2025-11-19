using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class EventConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> entity)
    {
        entity.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();
        entity.Property(e => e.StartDate)
            .HasColumnType("datetime2(0)")
            .IsRequired();
        entity.Property(e => e.EndDate)
            .HasColumnType("datetime2(0)")
            .IsRequired();
        entity.Property(e => e.CGI).HasMaxLength(200);
        entity.Property(e => e.Adress).HasMaxLength(200);
        entity.Property(e => e.Cost).HasDefaultValue(0.00);
        entity.Property(e => e.Capacity)
            .HasDefaultValue(0)
            .IsRequired();
        entity.Property(e => e.Description)
            .HasMaxLength(2000);
        entity.Property(e => e.Status)
            .IsRequired();

        // Indexes
        entity.HasIndex(e => e.StartDate);
        entity.HasIndex(e => e.Status);
    }
}

public class RegistrationConfig : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.UserSub)
              .IsRequired()
              .HasMaxLength(100);

        entity.Property(e => e.RegisteredAtUtc)
            .HasColumnType("datetime2(0)")
            .IsRequired();

        entity.HasOne(e => e.Event)
              .WithMany(ev => ev.EventRegistration)   
              .HasForeignKey(e => e.EventId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
