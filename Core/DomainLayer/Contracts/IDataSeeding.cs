using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IDataSeeding
    {
        Task IdentityDataSeedingAsync();
        Task SeedCustomersDataAsync();
    }
}
