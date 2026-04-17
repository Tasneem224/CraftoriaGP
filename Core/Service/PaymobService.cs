using Microsoft.Extensions.Configuration;
using ServiceAbstraction;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PaymobService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PaymobService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> CreatePaymentSession(OrderToPaymentDto dto)
        {
            // 1. Auth
            var authRes = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", new { api_key = _config["Paymob:ApiKey"] });
            var authData = await authRes.Content.ReadFromJsonAsync<dynamic>();
            string token = authData.token;

            // 2. Order
            var orderRes = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", new
            {
                auth_token = token,
                amount_cents = (long)(dto.TotalAmount * 100),
                currency = "EGP",
                items = new List<object>()
            });
            var orderData = await orderRes.Content.ReadFromJsonAsync<dynamic>();

            // 3. Payment Key
            var keyRes = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", new
            {
                auth_token = token,
                amount_cents = (long)(dto.TotalAmount * 100),
                order_id = orderData.id.ToString(),
                billing_data = new { email = dto.UserEmail, first_name = dto.FullName.Split(' ')[0], last_name = dto.FullName.Contains(' ') ? dto.FullName.Split(' ')[1] : "NA", phone_number = dto.Phone, city = dto.City, street = dto.Street, country = "EG", building = "NA", floor = "NA", apartment = "NA", state = "NA" },
                currency = "EGP",
                integration_id = int.Parse(_config["Paymob:IntegrationId"])
            });
            var keyData = await keyRes.Content.ReadFromJsonAsync<dynamic>();

            return $"https://accept.paymob.com/api/acceptance/iframes/{_config["Paymob:IframeId"]}?payment_token={keyData.token}";
        }
    }
}
