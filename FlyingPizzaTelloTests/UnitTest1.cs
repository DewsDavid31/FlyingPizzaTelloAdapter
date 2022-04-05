using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using FlyingPizzaTello;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using Moq.Protected;


namespace FlyingPizzaTelloTests;

public class Tests
{

    [Fact]
    public async Task ShouldRegisterWithDispatcher()
    {
        //TODO: mock httpclient to do direct invocation of adapter
        //TODO: mock drone behavior when not connected
        var testGuid = new Guid();
        var testIp = "http://tello/";
        var testDrone = new TelloAdapter(testGuid, new GeoLocation
        {
            Latitude = 0m,
            Longitude =0m
        });
        var mockHttpHandlerSetup = new Mock<HttpClientHandler>();
        mockHttpHandlerSetup.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(testDrone.InitRegistration().IsCompletedSuccessfully.ToString())
            });
        var mockHttpHandler = mockHttpHandlerSetup.Object;
        
        var testDroneInfo = new DroneRegistrationInfo
        {
            BadgeNumber = testGuid,
            IpAddress = testIp
        };
        var mockedDroneRepo = new Mock<IDronesRepository>().Object;
        var mockedOrderRepo = new Mock<IOrdersRepository>().Object;
        var testGateway = new DroneGateway();
        testGateway.changeHandler(mockHttpHandler);
        await mockedDroneRepo.CreateAsync(testDrone);
        var testDispatcher = new DispatcherController(mockedDroneRepo, mockedOrderRepo, testGateway);
        var response = await testDispatcher.RegisterNewDrone(testDroneInfo);
        var expected = new OkResult();
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expected);

    }
    
    [Fact]
    public async Task ShouldFinishInitWithDispatcher()
    {
        var client = new HttpClient();
        var values = new Dictionary<string, string> {};
        if (values == null) throw new ArgumentNullException(nameof(values));
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://localhost:5017/completregistration", content);
        var expected = new OkResult();
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task ShouldTakeOrder()
    {
        var client = new HttpClient();
        var values = new Dictionary<string, string>
        {
            {"latitude","98"},
            {"longitude","32"}
        };
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://localhost:5017/assignorder", content);
        var expected = new OkResult();
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task ShouldGoToProperGeo()
    {

        var ourAdapter = new TelloAdapter(new Guid(), new GeoLocation());
        var client = new HttpClient();
        var testLocation = new GeoLocation();
        testLocation.Latitude = 98;
        testLocation.Longitude = 32;
        var values = new Dictionary<string, string>
        {
            {"latitude","98"},
            {"longitude","32"}
        };
        var content = new FormUrlEncodedContent(values);
        await client.PostAsync("http://localhost:5017/assignorder", content);
        ourAdapter.Controller.Location.Should().NotBeNull();
        ourAdapter.Controller.Location.Should().BeEquivalentTo(testLocation);
    }
}