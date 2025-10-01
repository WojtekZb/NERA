using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configurations;

public class EventConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Title).IsRequired().HasMaxLength(200);
        e.Property(x => x.Date).IsRequired();
        e.Property(x => x.Location).HasMaxLength(200);
        e.Property(x => x.Cost).HasDefaultValue(0.00);
        e.Property(x => x.Capacity).HasDefaultValue(1).IsRequired();
        e.Property(x => x.Description).HasMaxLength(2000);
    }
}
