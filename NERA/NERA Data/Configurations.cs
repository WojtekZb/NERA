using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configurations;

public class EventConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> e)
    {
        e.Property(x => x.Title).IsRequired().HasMaxLength(200);
        e.Property(x => x.StartDate).IsRequired();
        e.Property(x => x.EndDate).IsRequired();
        e.Property(x => x.Location).HasMaxLength(200);
        e.Property(x => x.Cost).HasDefaultValue(0.00);
        e.Property(x => x.Capacity).HasDefaultValue(1).IsRequired();
        e.Property(x => x.Description).HasMaxLength(2000);
        e.Property(x => x.Status).IsRequired();
        e.HasIndex(x => x.StartDate);
        e.HasIndex(x => x.Status);
    }
}
public class UserConfig : IEntityTypeConfiguration<User>
{
   
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Name).IsRequired().HasMaxLength(50);
        e.HasIndex(x => x.Email).IsUnique();
        e.Property(x => x.Password).IsRequired().HasMaxLength(50);
        e.Property(x => x.Photo).HasMaxLength(100);
        e.Property(x => x.Type).IsRequired().HasMaxLength(20);
    }
}

public class RegistrationConfig : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> e)
    {
        e.HasKey(x => x.RegistrationId);
        e.Property(x => x.UserId).IsRequired();
        e.HasOne(x => x.User)
            .WithMany(u => u.Registration)
            .HasForeignKey(x => x.UserId);
        e.Property(x => x.EventId).IsRequired();
        e.HasOne(x => x.Event)
            .WithMany(ev => ev.Registration)
            .HasForeignKey(x => x.EventId);
        e.Property(x => x.Status).IsRequired().HasMaxLength(20);
        e.Property(x => x.Time).IsRequired();
    }
}
