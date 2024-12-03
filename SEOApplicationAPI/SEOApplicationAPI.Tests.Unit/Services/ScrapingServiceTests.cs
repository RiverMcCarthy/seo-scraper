using NUnit.Framework;
using NSubstitute;
using System.Net.Http;
using SEOApplicationAPI.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace SEOApplicationAPI.Tests.Unit.Services
{
    [TestFixture]
    public class ScrapingServiceTests
    {
        private ILogger<ScrapingService> _logger;
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            

            _logger = Substitute.For<ILogger<ScrapingService>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                {"AppSettings:NumberOfResults", "100"},
                {"AppSettings:BaseUrl", "https://www.google.com"},
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
        }

        [Test]
        public async Task ScrapeContent_SuccessfulRequest_ReturnsHtmlContent()
        {
            // Arrange
            var searchPhrase = "test";
            var expectedHtml = "<html><body>Test</body></html>";

            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, expectedHtml);

            var httpClient = new HttpClient(handler){};

            var scrapingService = new ScrapingService(httpClient, _logger, _configuration);

            // Act
            var result = await scrapingService.ScrapeContent(searchPhrase, CancellationToken.None);

            // Assert
            Assert.That(Regex.Unescape(result), Is.EqualTo($"\"{expectedHtml}\""));
        }

        [Test]
        public void ScrapeContent_RequestFails_ThrowsHttpRequestException()
        {
            // Arrange
            var searchPhrase = "test";

            var handler = new MockHttpMessageHandler(HttpStatusCode.BadRequest);

            var httpClient = new HttpClient(handler){};

            var scrapingService = new ScrapingService(httpClient, _logger, _configuration);

            // Act / Assert
            var ex = Assert.ThrowsAsync<HttpRequestException>(() => scrapingService.ScrapeContent(searchPhrase, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Request to https://www.google.com/search?num=100&q=test failed with status code BadRequest."));
        }
    }
}
