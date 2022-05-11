using Newtonsoft.Json;

namespace FlyingPizzaTello.DTO;

public class BaseDto
{
    [JsonProperty("Message")] public string Message { get; set; }
}