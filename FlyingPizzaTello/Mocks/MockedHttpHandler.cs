using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;

namespace FlyingPizzaTello.Mocks;

public class MockedHttpHandler
{
    private Mock<HttpMessageHandler> _handler;

    public MockedHttpHandler()
    {
        _handler = new Mock<HttpMessageHandler>();
    }

    public HttpMessageHandler createHandler()
    {
        return _handler.Object;
    }

    public void setResponseAssignDelivery(string uri, TelloAdapter adapter, GeoLocation arg)
    {
        _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri(uri)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(adapter.AssignDelivery(arg).Result.ToString())
            });

}
}