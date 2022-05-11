using System.Text.Json.Serialization;
using FlyingPizzaTello.Objects;

namespace FlyingPizzaTello.DTO.FrontEndDispatchCommunication;

public class AddDroneRequest : BaseDto
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("HomeLocation")] public GeoLocation HomeLocation { get; set; }

    [JsonPropertyName("DroneUrl")] public string DroneUrl { get; set; }

    [JsonPropertyName("DispatchUrl")] public string DispatchUrl { get; set; }
}