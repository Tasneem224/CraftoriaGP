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
    internal class DeliveryMethodConfigurationClass : IEntityTypeConfiguration<DeliveryMethod>
    {
      
            public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
            {
                builder.Property<decimal>(d => d.Price).HasColumnType("decimal(18,2)");

            }
        }
}
