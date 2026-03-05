namespace DomainLayer.Models.Order
{
  
    public enum OrderPaymentStatus
    {
        Pending,    
        PaymentReceived, 
        PaymentFailed,   
        Shipped,    
        Delivered   
    }

}