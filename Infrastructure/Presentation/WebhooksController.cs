using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceAbstraction;
using Stripe;
using System.IO;
using System.Threading.Tasks;

namespace Presentation
{
    // 1. خليناه public عشان الـ Runtime يشوفه
    // 2. ضفنا الـ Route والـ ApiController للتأكيد
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController(IOrderService _orderService, IConfiguration _config) : ControllerBase
    {
        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            // قراءة الـ Body بتاع الـ Request
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                // الـ Signature ده بيضمن إن اللي بيبعت هو Stripe مش حد بيخترق السيستم
                var stripeSignature = Request.Headers["Stripe-Signature"];
                var webhookSecret = _config["Stripe:WebhookSecret"];

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    webhookSecret
                );

                // 3. التعديل هنا: استخدام EventTypes أضمن وأوضح
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    if (paymentIntent != null)
                    {
                        // تنفيذ توزيع الـ 30% والـ 70% وتحديث حالة الأوردر
                        await _orderService.UpdateOrderStatusAndWallets(paymentIntent.Id);
                    }
                }

                // Stripe محتاج يستلم 200 OK عشان ميفضلش يبعت الإشارة تاني
                return Ok();
            }
            catch (StripeException e)
            {
                // لو فيه مشكلة في الـ Signature أو الداتا
                return BadRequest(e.Message);
            }
            catch (System.Exception ex)
            {
                // لأي خطأ غير متوقع
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}