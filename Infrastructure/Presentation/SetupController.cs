using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Controllers;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class SetupController:BaseApiController
    {
        private readonly IEtsyScrapperService _scrapperService;
        private static bool _isScrapingRunning = false;

        public SetupController(IEtsyScrapperService scrapperService)
        {
            _scrapperService = scrapperService;
        }

        [HttpPost("start-mega-scraping")]
        public IActionResult StartScraping([FromServices] IServiceScopeFactory scopeFactory)
        {
            if (_isScrapingRunning)
            {
                return BadRequest("السكرايبر شغال فعلاً في الخلفية.");
            }

            // تشغيل في Background Thread
            _ = Task.Run(async () =>
            {
                // بنكريت Scope جديد يدوي هنا
                using (var scope = scopeFactory.CreateScope())
                {
                    try
                    {
                        _isScrapingRunning = true;

                        // بنطلب السيرفيس من الـ Scope الجديد اللي كريتناه
                        var scrapperService = scope.ServiceProvider.GetRequiredService<IEtsyScrapperService>();

                        await scrapperService.StartScrapingAsync();
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine($"❌ Mega Scraping Stopped due to: {ex.Message}");
                    }
                    finally
                    {
                        _isScrapingRunning = false;
                        System.Console.WriteLine("🏁 Scraping Process Finished.");
                    }
                } // هنا الـ DbContext هيتمسح بس بعد ما السيرفيس تخلص شغلها تماماً
            });

            return Ok(new { Message = "🚀 عملية السحب بدأت بنجاح في Scope مستقل!" });
        }
    }
}
