using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    public enum PaymentStatus
    {
        Pending,     // قيد الانتظار
        Paid,        // تم الدفع
        Failed,      // فشل الدفع
        Refunded     // مسترد
    }
}
