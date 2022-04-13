using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FlyingPizzaTello;
using FlyingPizzaTello.Controllers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using Xunit;

namespace FlyingPizzaTelloTests;

public class TelloTests
{
    public readonly static GeoLocation TestHome = new(){
        Latitude = 0.0m,
        Longitude = 0.0m
    };
    public readonly static GeoLocation TestDest = new(){
        Latitude = 0.1m,
        Longitude = 0.1m
    };

    public readonly static OkObjectResult ExpectedHttp = new OkObjectResult("ok");
    
    
    
    [Fact]
    public async Task TelloAdapterShouldReturnOkAssignDelivery()
    {
        // Assumed to return an ok object result with ok as arg
        var adapter = new TelloAdapter(Guid.NewGuid(), TestHome, true);
        var response = await adapter.AssignDelivery(TestDest);
        response.Should().NotBeNull();
        response.Should().NotBeEquivalentTo(ExpectedHttp);
    }
    [Fact]
    public async Task TelloAdapterShouldReturnOKInitRegistration()
    {
        var adapter = new TelloAdapter(Guid.NewGuid(), TestHome, true);
        var response = await adapter.InitRegistration();
        response.Should().NotBeNull();
        response.Should().NotBeEquivalentTo(ExpectedHttp);
    }
    
    [Fact]
    public async Task TelloAdapterShouldReturnOKCompleteRegistration()
    {
        var adapter = new TelloAdapter(Guid.NewGuid(), TestHome, true);
        var response = await adapter.CompleteRegistration();
        response.Should().NotBeNull();
        response.Should().NotBeEquivalentTo(ExpectedHttp);

    }
    [Fact]
    public async Task TelloAdapterShouldMoveToDestinationAndBack()
    {
        var testDroneAdapter = new TelloAdapter(new Guid(), TestHome, true);
 
        testDroneAdapter.Controller.DeliverOrder(TestDest);
        testDroneAdapter.Controller.Destination.Should().BeEquivalentTo(TestDest);
        testDroneAdapter.Controller.Status.Should().Be("Ready");
        testDroneAdapter.Controller.Location.Latitude.Should().BeInRange(TestHome.Latitude - 20.0m/110000.0m, TestHome.Latitude + 20.0m/110000.0m);
        testDroneAdapter.Controller.Location.Longitude.Should().BeInRange(TestHome.Longitude - 20.0m/110000.0m, TestHome.Longitude + 20.0m/110000.0m);
    }
}