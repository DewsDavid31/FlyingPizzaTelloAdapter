using FlyingPizzaTello.DTO;
using FlyingPizzaTello.DTO.DroneDispatchCommunication;
using FlyingPizzaTello.Entities;

namespace FlyingPizzaTello.GatewayDefinitions;

public interface IDroneToDispatchGateway
{
    public string GetEndPoint();
    public void ChangeHandler(HttpMessageHandler handler);

    public Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneUpdate request);

    public Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request);

    public Task<bool> Revive(DroneRecord record);
}