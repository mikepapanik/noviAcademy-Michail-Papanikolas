using System.Net;

namespace WorldRank.UnitTests.Helpers;

public sealed class StubHttpMessageHandler
    : HttpMessageHandler
{
    private readonly Func<
        HttpRequestMessage,
        CancellationToken,
        Task<HttpResponseMessage>> _responseFactory;

    public StubHttpMessageHandler(
        Func<
            HttpRequestMessage,
            CancellationToken,
            Task<HttpResponseMessage>> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public HttpRequestMessage? LastRequest { get; private set; }

    public int CallCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        CallCount++;

        return _responseFactory(
            request,
            cancellationToken);
    }

    public static StubHttpMessageHandler Create(
        HttpStatusCode statusCode,
        string content)
    {
        return new StubHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(
                    statusCode)
                {
                    Content = new StringContent(content)
                };

                return Task.FromResult(response);
            });
    }
}
