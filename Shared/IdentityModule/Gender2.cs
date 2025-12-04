using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.IdentityModule
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum Gender2
    {
        [EnumMember(Value = "Female")]
        Female = 1,
        [EnumMember(Value = "Male")]
        Male = 2,
    }
}
