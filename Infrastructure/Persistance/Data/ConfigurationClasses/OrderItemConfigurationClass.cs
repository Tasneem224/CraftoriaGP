using DomainLayer.Models.Order;
using Google.Apis.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    internal class OrderItemConfigurationClass : BaseEntityConfigurationClass<OrderItem, int>
    {
        
        public  override void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            base.Configure(builder); 
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id)
                   .UseIdentityColumn(); // يجبر SQL Server على توليد الرقم تلقائياً
            //builder.Property<decimal>(o => o.Price).HasColumnType("decimal(18,2)");
            builder.Property<decimal>(o => o.Price).HasColumnType("decimal(18,2)");
            builder.OwnsOne(o => o.Item, item =>
            {
                item.WithOwner(); // ده بيأكد إنه تابع للـ OrderItem وموش محتاج Key لوحده
            });
        }

    }
}
