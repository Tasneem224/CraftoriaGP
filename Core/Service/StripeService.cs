using Microsoft.Extensions.Configuration;
using ServiceAbstraction;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Stripe.Checkout;
namespace Service
{
    public class StripeService : IPaymentService
    {
        private readonly IConfiguration _config;
        public StripeService(IConfiguration config) => _config = config;

        public async Task<string> CreatePaymentSession(OrderToPaymentDto dto)
        {
            Stripe.StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = dto.Items.Select(i => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(i.Price * 100),
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = i.ProductName }
                    },
                    Quantity = i.Quantity
                }).ToList(),
                Mode = "payment",
                SuccessUrl = _config["Stripe:SuccessUrl"],
                CancelUrl = _config["Stripe:CancelUrl"],
                CustomerEmail = dto.UserEmail
            };
            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }
    }
}
