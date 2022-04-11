using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FlyingPizzaTello;
using FlyingPizzaTello.Mocks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using Xunit;

namespace FlyingPizzaTelloTests;

public class TelloTests
{
    [Fact]
    public async Task TelloAdapterShouldAssignDelivery()
    {
        Assert.Fail("Not implemented");
    }
    [Fact]
    public async Task TelloAdapterShouldInitRegistration()
    {
        
        Assert.Fail("Not implemented");
    }
    [Fact]
    public async Task TelloAdapterShouldCompleteRegistration()
    {
        
        Assert.Fail("Not implemented");
    }
    [Fact]
    public async Task TelloAdapterShouldMoveToDestinationAndBack()
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
        var testDroneAdapter = new TelloAdapter(new Guid(), testHome, true);
 
        var testHttpClient = new HttpClient();
        var responseMessage = await testHttpClient.PostAsync("http://localhost:5017/assignDelivery", new StringContent(testDestination.ToJson()));
        var expected = new OkResult();
        responseMessage.Should().NotBeNull();
        responseMessage.Should().BeEquivalentTo(expected);
    }
}