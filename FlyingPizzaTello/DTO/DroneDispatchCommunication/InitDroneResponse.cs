using System.Text.Json.Serialization;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class InitDroneResponse : BaseDto
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("Okay")] public bool Okay { get; set; }
}