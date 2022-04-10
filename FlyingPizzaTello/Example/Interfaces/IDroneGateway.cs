
using System;
using System.Threading.Tasks;
using FlyingPizzaTello;

//TODO: add this DTO

namespace FlyingPizzaTelloTests;


public interface IDroneGateway
{
    public Task<bool> StartRegistration(string droneIpAddress, Guid badgeNumber, string 
        dispatcherUrl, GeoLocation homeLocation);

    public Task<bool> AssignDelivery(string droneIpAddress, string orderNumber, GeoLocation orderLocation);

    public Task<bool> OKToSendStatus(string droneIpAddress);
}