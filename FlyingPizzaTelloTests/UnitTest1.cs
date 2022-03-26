using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using FlyingPizzaTello;

namespace FlyingPizzaTelloTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task ShouldRegisterWithDispatcher()
    {
        var client = new HttpClient();
        var values = new Dictionary<string, string> {};
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://localhost:5017/initregistration", content);
        Assert.AreEqual(response.IsSuccessStatusCode, true);

    }
    
    [Test]
    public async Task ShouldFinishInitWithDispatcher()
    {
        var client = new HttpClient();
        var values = new Dictionary<string, string> {};
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://localhost:5017/completregistration", content);
        Assert.AreEqual(response.IsSuccessStatusCode, true);
    }
    
    [Test]
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
        Assert.AreEqual(response.IsSuccessStatusCode, true);
    }
    
    [Test]
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
        Assert.AreEqual(ourAdapter.Controller.Location, testLocation);
    }
}