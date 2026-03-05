using DomainLayer.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    internal class OrderItemConfigurationClass : IEntityTypeConfiguration<OrderItem>
    {
      
            public void Configure(EntityTypeBuilder<OrderItem> builder)
            {
                builder.Property<decimal>(o => o.Price).HasColumnType("decimal(18,2)");
                builder.OwnsOne(o => o.Item, p => p.WithOwner());
            }
        }
}
