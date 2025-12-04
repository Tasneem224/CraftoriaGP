using DomainLayer.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    public class IdentitiyConfigurationClass : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(e => e.Gender)
                   .HasConversion<string>((gender) => gender.ToString(),
                ToGender => (Gender)Enum.Parse(typeof(Gender), ToGender));
            builder.Property(e => e.FirstName)
                .HasColumnType("nvarchar(50)");
            builder.Property(e => e.SecondName)
                .HasColumnType("nvarchar(50)");
            builder.Property(e => e.Bio)
                .HasColumnType("nvarchar(250)");

        }
    }
}
