using System.Text.Json.Serialization;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class InitDroneRequest : BaseDto
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("DroneUrl")] public string DroneUrl { get; set; }
}