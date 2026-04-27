using DomainLayer.Models.Community_Space;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    public class PostLovesConfiguration : IEntityTypeConfiguration<PostLoves>
    {
        public void Configure(EntityTypeBuilder<PostLoves> builder)
        {

            builder.HasIndex(pl => new { pl.PostId, pl.UserId })
                .IsUnique();

            builder.HasOne(pl => pl.Post)
                .WithMany(p => p.Loves)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pl => pl.User)
                .WithMany()
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
