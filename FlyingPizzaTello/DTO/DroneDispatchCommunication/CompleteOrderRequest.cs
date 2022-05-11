using System.Text.Json.Serialization;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class CompleteOrderRequest : BaseDto
{
    [JsonPropertyName("OrderId")] public string OrderId { get; set; }

    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("Time")] public DateTime Time { get; set; }
}