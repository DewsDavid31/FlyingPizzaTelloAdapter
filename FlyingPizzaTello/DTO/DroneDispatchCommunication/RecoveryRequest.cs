using Domain.Entities;
using FlyingPizzaTello.Entities;

namespace FlyingPizzaTello.DTO.DroneDispatchCommunication;

public class RecoveryRequest : BaseDto
{
    public DroneRecord Record { get; set; } 
}