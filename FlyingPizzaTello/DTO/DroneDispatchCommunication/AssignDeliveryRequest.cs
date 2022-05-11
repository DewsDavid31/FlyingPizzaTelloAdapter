﻿using System.Text.Json.Serialization;
using FlyingPizzaTello.Objects;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class AssignDeliveryRequest : BaseDto
{
    [JsonPropertyName("OrderId")] public string OrderId { get; set; }

    [JsonPropertyName("OrderLocation")] public GeoLocation OrderLocation { get; set; }

    [JsonPropertyName("DroneId")] public string DroneId { get; set; }
}