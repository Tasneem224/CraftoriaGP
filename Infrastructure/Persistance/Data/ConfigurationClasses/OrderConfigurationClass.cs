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
    public class OrderConfigurationClass : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
                builder.OwnsOne(o => o.ShippingAddress,a=>a.WithOwner() );
                
             
                //builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
                builder.Property(o => o.Subtotal).HasColumnType("numeric(18,2)");
                builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

                builder.Property(o => o.orderPaymentStatus).HasConversion(p => p.ToString(), p => Enum.Parse<OrderPaymentStatus>(p));
                builder.Property(o => o.orderStatus).HasConversion(p => p.ToString(), p => Enum.Parse<OrderStatus>(p));

                builder
                .HasOne(o => o.DeliveryMethod)
                .WithMany()
                .HasForeignKey(o => o.DeliveryMethodId)
                .OnDelete(DeleteBehavior.SetNull);
            }
      
        }
    }

