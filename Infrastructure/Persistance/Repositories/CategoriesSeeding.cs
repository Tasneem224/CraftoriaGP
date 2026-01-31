using DomainLayer.Contracts;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class CategoriesSeeding : ICategoriesSeeding
    {
        private readonly IWebHostEnvironment _env;

        private readonly StoreDbContext _storeDbContext;
        
        public CategoriesSeeding(StoreDbContext storeDbContext, IWebHostEnvironment env) // <- constructor injection
        {
            _storeDbContext = storeDbContext ?? throw new ArgumentNullException(nameof(storeDbContext));
            _env = env;
        }

        public async Task CategoryDataSeedingAsync()
        {

            try
            {
                if (!_storeDbContext.Categories.Any())
                {

                 
                    if (!_storeDbContext.Categories.AsNoTracking().Any())
                    {
                        string filePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "Seeding data Files",
                        "categories.data.json"
                    );

                        string categoryJson = File.ReadAllText(filePath);
                        List<Category>? categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);

                        if (categories != null && categories.Any())
                        {
                            await _storeDbContext.Categories.AddRangeAsync(categories);
                            await _storeDbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error seeding categories: {ex.Message}");
            }

        }
    }
}
