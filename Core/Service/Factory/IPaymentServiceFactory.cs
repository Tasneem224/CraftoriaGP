using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Factory
{
    public interface IPaymentServiceFactory
    {
        public IPaymentService GetPaymentService(string method);
    }
}
