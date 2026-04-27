using DomainLayer.Models.Community_Space;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    public class PostConfigurationClass : IEntityTypeConfiguration<Post>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(500);

            builder.HasOne(p => p.Author)
            .WithMany() 
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
