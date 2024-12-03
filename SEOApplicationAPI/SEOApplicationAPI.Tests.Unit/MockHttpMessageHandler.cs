using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SEOApplicationAPI.Tests.Unit
{
    public class MockHttpMessageHandler(HttpStatusCode statusCode, object? responseContent = null) : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode = statusCode;
        private readonly object? _responseContent = responseContent;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = JsonContent.Create(_responseContent)
            });
        }
    }
}