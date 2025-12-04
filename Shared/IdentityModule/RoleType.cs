using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityModule
{
    public enum RoleType
    {
        [EnumMember(Value = "Beginner")]
        Beginner = 1,
        [EnumMember(Value = "Customer")]
        Customer = 2,
        [EnumMember(Value = "Supplier")]
        Supplier = 3,
        [EnumMember(Value = "Expert")]
        Expert = 4
    }
}
