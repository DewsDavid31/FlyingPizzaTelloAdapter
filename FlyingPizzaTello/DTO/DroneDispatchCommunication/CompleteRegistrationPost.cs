using System.Text.Json.Serialization;
using Domain.Entities;
using FlyingPizzaTello.Entities;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest : BaseDto
{
    [JsonPropertyName("Record")] public DroneRecord Record { get; set; }
}