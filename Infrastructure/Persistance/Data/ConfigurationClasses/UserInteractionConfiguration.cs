using DomainLayer.Models.Interaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    internal class UserInteractionConfiguration 
    {
        //public void Configure(EntityTypeBuilder<UserInteraction> builder)
        //{
        //    builder.HasKey(x => x.Id);

        //    builder.Property(x => x.UserId)
        //           .IsRequired();
            

        //    builder.Property(x => x.TargetId)
        //           .IsRequired();

        //    builder.Property(x => x.TargetType)
        //           .IsRequired();

        //    builder.HasIndex(x => new
        //    {
        //        x.UserId,
        //        x.TargetId,
        //        x.TargetType
        //    })
        //    .IsUnique();

        //    builder.Property(x => x.InteractionDate)
        //           .HasDefaultValueSql("GETUTCDATE()");
        //}
    }
}
