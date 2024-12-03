using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using SEOApplicationAPI.Controllers;
using SEOApplicationAPI.Interfaces;
using SEOApplicationAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SEOApplicationAPI.Tests.Unit.Controllers
{
    [TestFixture]
    public class SEOControllerTests
    {
        private SEOController _controller;
        private IScrapingService _scrapingService;
        private IRankingService _rankingService;
        private ILogger<SEOController> _logger;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _scrapingService = Substitute.For<IScrapingService>();
            _rankingService = Substitute.For<IRankingService>();
            _logger = Substitute.For<ILogger<SEOController>>();
            _httpContext = new DefaultHttpContext();

            _controller = new SEOController(_scrapingService, _rankingService, _logger)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext,
                }
            };
        }

        [Test]
        public async Task ScrapeGoogle_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var searchPhrase = "test";
            var targetUrl = "https://test.com";
            var html = "<html><a></a></html>";
            var rankings = new List<int> { 1, 2, 3 };

            _scrapingService.ScrapeContent(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(html));

            _rankingService.GetRankingsAsync(Arg.Any<string>(), Arg.Any<string>(), targetUrl)
                .Returns(Task.FromResult(rankings));

            // Act
            var result = await _controller.ScrapeGoogle(searchPhrase, targetUrl);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.EqualTo("1, 2, 3"));
            });
        }

        [Test]
        public async Task ScrapeGoogle_MissingSearchPhrase_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.ScrapeGoogle("", "https://test.com");

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
                Assert.That(badRequestResult.Value, Is.EqualTo("A search phrase and target URL must be provided."));
            });
        }

        [Test]
        public async Task ScrapeGoogle_MissingTargetUrl_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.ScrapeGoogle("test", "");

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
                Assert.That(badRequestResult.Value, Is.EqualTo("A search phrase and target URL must be provided."));
            });
        }

        [Test]
        public async Task ScrapeGoogle_ExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            var searchPhrase = "test";
            var targetUrl = "https://test.com";

            _scrapingService.ScrapeContent(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Throws(new Exception());

            // Act
            var result = await _controller.ScrapeGoogle(searchPhrase, targetUrl);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.That(internalServerErrorResult, Is.Not.Null);
            Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetAllRankings_IsSuccessful_ReturnsOkResult()
        {
            // Arrange
            var rankings = new Rankings
            {
                Ranks =
                [
                    new Ranking { SearchPhrase = "test1", Url = "https://test1.com", Rank = 1, SearchedOn = DateTime.Now },
                    new Ranking { SearchPhrase = "test2", Url = "https://test2.com", Rank = 2, SearchedOn = DateTime.Now }
                ]
            };

            _rankingService.GetAllRankingsAsync().Returns(Task.FromResult(rankings));

            // Act
            var result = await _controller.GetAllRankings();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.EqualTo(rankings));
            });
        }

        [Test]
        public async Task GetAllRankings_ExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _rankingService.GetAllRankingsAsync().Throws(new Exception("Error occurred"));

            // Act
            var result = await _controller.GetAllRankings();

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.That(internalServerErrorResult, Is.Not.Null);
            Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500));
        }
    }
}
