using System.Runtime.Serialization;

namespace DomainLayer.Models.Order
{
  
    public enum OrderPaymentStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,          
        [EnumMember(Value = "Received")]
        PaymentReceived,  
        [EnumMember(Value = "Failed")]
        PaymentFailed,     
        [EnumMember(Value = "Refunded")]
        Refunded         
    }

    }