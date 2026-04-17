using System.Runtime.Serialization;

namespace DomainLayer.Models.Order
{
  
    public enum OrderPaymentStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,          
        [EnumMember(Value = "Confirmed")]
        Confirmed,          
        [EnumMember(Value = "Received")]
        Received,            
        [EnumMember(Value = "Failed")]
        PaymentFailed,     
        [EnumMember(Value = "Refunded")]
        Refunded         
    }

    }