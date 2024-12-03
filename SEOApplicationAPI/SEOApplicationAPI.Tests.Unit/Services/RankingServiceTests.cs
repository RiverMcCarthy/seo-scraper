using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SEOApplicationAPI.DataAccessLayer.Contexts;
using SEOApplicationAPI.DataAccessLayer.Models;
using SEOApplicationAPI.Helpers;
using SEOApplicationAPI.Models;
using SEOApplicationAPI.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using SEOApplicationAPI.Mappers;

namespace SEOApplicationAPI.Tests.Unit.Services
{
    [TestFixture]
    public class RankingServiceTests
    {
        private RankingService _rankingService;
        private SEODbContext _dbContext;
        private IMapper _mapper;
        private ILogger<RankingService> _logger;
        private ILogger<ParsingHelper> _parsingLogger;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<SEODbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new SEODbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RankingMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _logger = Substitute.For<ILogger<RankingService>>();
            _parsingLogger = Substitute.For<ILogger<ParsingHelper>>();

            _rankingService = new RankingService(_logger, _parsingLogger, _dbContext, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext?.Dispose();
        }

        [Test]
        public async Task GetRankingsAsync_FindsTargetUrl_ReturnsListWithRanking()
        {
            // Arrange
            var html = "<html><a jsname=\"UWckNb\" href=\"https://test.com\"></a></html>";
            var searchPhrase = "test";
            var targetUrl = "https://test.com";

            // Act
            var result = await _rankingService.GetRankingsAsync(html, searchPhrase, targetUrl);

            // Assert
            Assert.That(result, Is.EqualTo(new List<int> { 1 }));
        }

        [Test]
        public async Task GetRankingsAsync_TargetUrlNotFound_ReturnsListWithZero()
        {
            // Arrange
            var html = "<html><a href='https://result.com'></a></html>";
            var searchPhrase = "test";
            var targetUrl = "https://test.com";

            // Act
            var result = await _rankingService.GetRankingsAsync(html, searchPhrase, targetUrl);

            // Assert
            Assert.That(result, Is.EqualTo(new List<int> { 0 }));
        }

        [Test]
        public async Task GetAllRankingsAsync_ReturnsRankings()
        {
            // Arrange
            var mockRankings = new List<RankingDbo>
            {
                new() { SearchPhrase = "test1", Url = "https://test1.com", Rank = 1, SearchedOn = DateTime.Now },
                new() { SearchPhrase = "test2", Url = "https://test2.com", Rank = 2, SearchedOn = DateTime.Now }
            };

            _dbContext.Rankings.AddRange(mockRankings);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _rankingService.GetAllRankingsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Rankings>());
            Assert.That(result.Ranks?.ToList(), Has.Count.EqualTo(2));
        }
    }
}
