﻿using System.Text.Json.Serialization;
using FlyingPizzaTello.Objects;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class UpdateDroneStatusRequest
{
    [JsonPropertyName("DroneId")]
    public string DroneId{get;set;}

    [JsonPropertyName("Location")]
    public GeoLocation Location { get; set; } 
        
    [JsonPropertyName("State")]
    public DroneState State { get; set; }
}