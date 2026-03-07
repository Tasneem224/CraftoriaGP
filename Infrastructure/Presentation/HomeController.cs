using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using Service;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class HomeController : BaseApiController
    {
        private readonly IServiceManager  _serviceManager;

        // بنعمل Inject للسرفيس الجديدة بتاعتنا
        public HomeController( IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // 1. Endpoint للمنتجات الأعلى تقييماً
        // GET: api/Home/top-products?count=5
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int count = 5)
        {
            var result = await _serviceManager.TopRatedService.GetTopProductsAsync(count);

            // لو القائمة فاضية ممكن ترجعي 200 برضه عادي (لستة فاضية) أو 404 حسب البيزنس
            return Ok(result);
        }

        // 2. Endpoint للبائعين الأعلى تقييماً
        // GET: api/Home/top-sellers?count=5
        [HttpGet("top-sellers")]
        public async Task<IActionResult> GetTopSellers([FromQuery] int count = 5)
        {
            var result = await _serviceManager.TopRatedService.GetTopSellersAsync(count);
            return Ok(result);
        }
        [HttpGet("top-raw-materials")]
        public async Task<IActionResult> GetTopRawMaterials([FromQuery] int count = 5)
        {
            var result = await _serviceManager.TopRatedService.GetTopRawMaterialsAsync(count);
            return Ok(result);
        }
        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Search query cannot be empty");

            var results = await _serviceManager.ProductService.SearchProductsAsync(query);
            return Ok(results);
        }



    }
}
