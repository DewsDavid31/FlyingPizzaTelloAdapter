﻿using System.Text.Json.Serialization;

namespace FlyingPizzaTello.DTO.FrontEndDispatchCommunication;

public class AddDroneResponse : BaseDto
{
    [JsonPropertyName("Success")] public bool Success { get; set; }

    [JsonPropertyName("DroneId")] public string DroneId { get; set; }
}