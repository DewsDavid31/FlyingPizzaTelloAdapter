using System.Text.Json.Serialization;

namespace FlyingPizzaTello.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryRequest : BaseDto
{
    [JsonPropertyName("OrderId")] public string OrderId { get; set; }
}