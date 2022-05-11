using System.Text.Json.Serialization;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class CompleteOrderResponse : BaseDto
{
    [JsonPropertyName("IsAcknowledged")] public bool IsAcknowledged { get; set; }
}