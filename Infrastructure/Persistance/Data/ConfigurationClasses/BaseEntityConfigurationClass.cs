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
    public abstract class BaseEntityConfigurationClass<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.CreatedAt)
                           .HasDefaultValueSql("GETDATE()"); 

            builder.Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.CreatedBy)
                   .HasMaxLength(150);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
