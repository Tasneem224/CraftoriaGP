using DomainLayer.Contracts;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Items;
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
        private readonly StoreDbContext _storeDbContext;
       
        public async Task CategoryDataSeedingAsync()
        {

            try
            {
                string categoryJson= File.ReadAllText("categories.data.json");
                List<Category>? categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);

                if (!_storeDbContext.Categories.Any())
                {
                    await _storeDbContext.Categories.AddRangeAsync(categories);
                    await _storeDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error seeding categories: {ex.Message}");
            }

        }
    }
}
