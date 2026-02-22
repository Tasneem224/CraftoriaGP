using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [Authorize]
    public class FavouritesController : BaseApiController
    {
        private readonly IFavouriteService _favService;

        public FavouritesController(IFavouriteService favService)
        {
            _favService = favService;
        }


        [HttpPost("Toggle")]
        public async Task<IActionResult> Toggle([FromQuery] int productId)
        {
            // بنجيب الـ ID من التوكن عشان الأمان
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _favService.ToggleFavouriteAsync(userId, productId);
            return Ok(new { message = result });
        }

        [HttpGet("MyFavourites")]
        public async Task<IActionResult> GetMyFavourites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _favService.GetUserFavouritesAsync(userId);
            return Ok(result);
        }

    }
}
