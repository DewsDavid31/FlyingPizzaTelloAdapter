using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FlyingPizzaTello;
using FlyingPizzaTello.Mocks;
using Moq;
using Moq.Protected;
using Xunit;

namespace FlyingPizzaTelloTests;

public class TelloTests
{
    
    // register the tello
    
    // make tello deliver
    
    // tello will deliver to location
    [Fact]
    public async Task tello_should_deliver_to_location()
    {
        var testHome = new GeoLocation
        {
            Latitude = 0.0m,
            Longitude = 0.0m
        };
        var testDelivery = new GeoLocation
        {
            Latitude = 0.001m,
            Longitude = 0.001m
        };
        var testDroneInfo = new DroneRegistrationInfo
        {
            BadgeNumber = new Guid(),
            IpAddress = "192.168.10.1"
        };
        var mockedDroneSetup = new Mock<Tello>();
        var mockedHardware = new MockedTello(testHome);
        mockedDroneSetup.Setup(x => x.send_command(It.IsAny<string>())).Returns<bool>(x => mockedHardware.send_command(It.IsAny<string>()));
        var mockedDrone = mockedDroneSetup.Object;
        var testDrone = new TelloAdapter(testDroneInfo.BadgeNumber,testHome,mockedDrone);
        var mockedHttpHandlerSetup = new MockedHttpHandler();
        mockedHttpHandlerSetup.setResponseAssignDelivery("http://192.168.10.1/drone/assigndelivery",testDrone, testDelivery);
        var mockedHttpHandler = mockedHttpHandlerSetup.createHandler();
        var testGateway = new DroneGateway();
        testGateway.changeHandler(mockedHttpHandler);
        var result = await testGateway.AssignDelivery(testDroneInfo.IpAddress, "undefined order", testDelivery);
        result.Should().BeTrue();

    }
}