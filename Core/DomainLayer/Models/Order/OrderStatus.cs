using System.Runtime.Serialization;

namespace DomainLayer.Models.Order
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,          
        [EnumMember(Value = "InPreparation")]
        InPreparation,    
        [EnumMember(Value = "Confirmed")]
        Confirmed,
        [EnumMember(Value = "Received")]
        Received,
        [EnumMember(Value = "Shipped")]
        Shipped,          
        [EnumMember(Value = "Delivered")]
        Delivered,        
        [EnumMember(Value = "Cancelled")]
        Cancelled         
    }

}