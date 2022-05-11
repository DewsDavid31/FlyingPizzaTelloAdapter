using System.Text.Json.Serialization;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class AssignFleetResponse : BaseDto
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("IsInitializedAndAssigned")]
    public bool IsInitializedAndAssigned { get; set; }

    [JsonPropertyName("FirstState")] public DroneState FirstState { get; set; }
}