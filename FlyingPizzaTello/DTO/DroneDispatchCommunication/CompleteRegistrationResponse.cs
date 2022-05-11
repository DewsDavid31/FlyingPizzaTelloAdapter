using System.Text.Json.Serialization;
using Domain.Entities;
using FlyingPizzaTello.Entities;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class CompleteRegistrationResponse : BaseDto
{
    [JsonPropertyName("Record")] public DroneRecord Record { get; set; }

    [JsonPropertyName("Okay")] public bool Okay { get; set; }
}