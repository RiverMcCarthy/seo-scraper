using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;
using SEOApplicationAPI.Interfaces;
using SEOApplicationAPI.Services;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SEOApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SEOController(
        IScrapingService scrapingService,
        IRankingService rankingService,
        ILogger<SEOController> logger)
            : ControllerBase
    {
        private readonly IScrapingService _scrapingService = scrapingService;
        private readonly IRankingService _rankingService = rankingService;
        private readonly ILogger<SEOController> _logger = logger;

        /// <summary>
        /// Scrapes data using the search phrase provided.
        /// </summary>
        /// <param name="searchPhrase">The search phrase to append to the base url.</param>
        /// <param name="targetUrl">The target URL to return ranks for.</param>
        /// <returns>A list of ranks which the targetUrl was found in the search results.</returns>
        [HttpGet("scrape-google")]
        public async Task<IActionResult> ScrapeGoogle([FromQuery] string searchPhrase, [FromQuery] string targetUrl)
        {
            if (string.IsNullOrEmpty(searchPhrase) || string.IsNullOrEmpty(targetUrl))
                return BadRequest("A search phrase and target URL must be provided.");

            try
            {
                var html = await _scrapingService.ScrapeContent(searchPhrase, HttpContext.RequestAborted);
                var rankings = await _rankingService.GetRankingsAsync(html, searchPhrase, targetUrl);

                return Ok(string.Join(", ", rankings));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the search request.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get ranks from the database.
        /// </summary>
        /// <returns>A list of ranks which the targetUrl was found in the search results.</returns>
        [HttpGet("get-rankings")]
        public async Task<IActionResult> GetAllRankings()
        {
            try
            {
                var rankingData = await _rankingService.GetAllRankingsAsync();

                return Ok(rankingData);
            }
                catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
