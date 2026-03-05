using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    public enum SessionStatus
    {
        Pending,     // قيد الانتظار
        Confirmed,   // مؤكدة
        Completed,   // مكتملة
        Cancelled    // ملغية
    }
}
