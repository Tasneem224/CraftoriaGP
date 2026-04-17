using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using System;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(IOrderService _orderService) : ControllerBase
    {
        [HttpPost("{orderId}/{method}")]
        public async Task<IActionResult> CreatePayment(Guid orderId, string method)
        {
            try
            {
                // الـ Controller ميعرفش أي تفاصيل عن الـ Mapping أو الـ Factory
                // هو بيسأل الـ Service: "خدي الـ ID ده وخليني أدفع بالطريقة دي"
                var paymentUrl = await _orderService.CreatePaymentAsync(orderId, method);

                return Ok(new { url = paymentUrl });
            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء بشكل مركزي
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("success")]
        public IActionResult PaymentSuccess() => Ok("Payment successful!");

        [HttpGet("cancel")]
        public IActionResult PaymentCancel() => BadRequest("Payment canceled.");
    }
}