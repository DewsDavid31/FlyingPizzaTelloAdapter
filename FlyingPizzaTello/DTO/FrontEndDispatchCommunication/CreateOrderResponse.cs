namespace FlyingPizzaTello.DTO.FrontEndDispatchCommunication;

public class CreateOrderResponse : BaseDto
{
    public string OrderId { get; set; }
    public bool IsCompletedSuccessfullly { get; set; }
}