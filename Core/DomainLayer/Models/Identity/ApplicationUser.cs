using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Identity
{
    public class ApplicationUser : IdentityUser

    {

        public string DisplayName { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string SecondName { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public string? ProfileImage { get; set; } = default!;
        public string? Bio { get; set; } = string.Empty;
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiration { get; set; }

        #region Artisan
        public decimal? CommissionRate { get; set; }//by default is 10%
        public string? Specialization { get; set; } = default!;
        #endregion

        #region Expert_artisan
        public string? Portfolio { get; set; }
        public int? YearsOfExperience { get; set; }

        #endregion

        // virtual for lazy loading
        [InverseProperty(nameof(Product.Seller))]
        public virtual ICollection<Product>? Products { get; set; }

        [InverseProperty(nameof(RawMaterial.supplier))]
        public virtual ICollection<RawMaterial>? rawMaterials { get; set; }



    }
}
