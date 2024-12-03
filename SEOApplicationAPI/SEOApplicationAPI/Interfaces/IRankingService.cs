using SEOApplicationAPI.Models;

namespace SEOApplicationAPI.Interfaces
{
    public interface IRankingService
    {
        /// <summary>
        /// Gets the rankings of the target url based on the returned search responses.
        /// </summary>
        /// <param name="html">The HTML response.</param>
        /// <param name="searchPhrase">The search phrase.</param>
        /// <param name="targetUrl">The URL to rank.</param>
        /// <returns>A list of all ranks where the targetUrl is found in the response results.</returns>
        public Task<List<int>> GetRankingsAsync(string html, string searchPhrase, string targetUrl);

        /// <summary>
        /// Gets all ranking data from the database.
        /// </summary>
        /// <returns></returns>
        public Task<Rankings> GetAllRankingsAsync();
    }
}
