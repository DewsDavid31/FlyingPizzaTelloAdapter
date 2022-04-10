using System;
using System.Threading.Tasks;
using FluentAssertions;
using FlyingPizzaTello;
using FlyingPizzaTello.Mocks;
using Moq;
using Xunit;

namespace FlyingPizzaTelloTests;

public class TelloTests
{
    [Fact]
    public async Task TelloAdapterShouldAssignDelivery()
    {
        var testHome = new GeoLocation
        {
            Latitude = 0m,
            Longitude = 0m
        };
        var testDestination = new GeoLocation
        {
            Latitude = 0.001m,
            Longitude = 0.001m
        };
        var mockedHttpHandlerFactory = new MockedHttpHandler();
        var testTelloSetup = new Mock<MockedTello>(testHome);
        var testTello = testTelloSetup.Object;
        var adapter = new TelloAdapter(new Guid(), testHome, testTello);
        mockedHttpHandlerFactory.setResponseAssignDelivery("http://192.168.10.1/drone/assigndelivery", adapter, testDestination);
        var testGateway = new DroneGateway();
        testGateway.changeHandler(mockedHttpHandlerFactory.createHandler());
        var response = await testGateway.AssignDelivery("192.168.10.1", "0", testDestination);
        response.Should().BeTrue();
        testTello.response.Should().Be("OK");
        testTello.current.Should().NotBeNull();
        testTello.current.Should().BeEquivalentTo(testHome);
        //TODO: this overrides send_command of our mocked drone, disallowing normal return of "OK"
        testTelloSetup.Verify(x => x.send_command(It.IsAny<string>()));
    }
}