using Microsoft.Extensions.DependencyInjection;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Factory
{
    public class PaymentServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentService GetPaymentService(string method)
        {
            return method.ToLower() switch
            {
              
                "stripe" => _serviceProvider.GetRequiredService<StripeService>(),
                "paymob" => _serviceProvider.GetRequiredService<PaymobService>(),

                _ => throw new Exception("طريقة الدفع دي مش موجودة")
            };
        }
    }
}
