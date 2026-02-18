using DomainLayer.Models.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    public class TagConfigurationClass : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasIndex(t=> t.Name).IsUnique();

            builder.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.HasMany(i => i.items)
                    .WithMany(t => t.tags)
                    .UsingEntity(j => j.ToTable("ItemTags"));
        }
    }
}
