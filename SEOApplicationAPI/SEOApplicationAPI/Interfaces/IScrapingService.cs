namespace SEOApplicationAPI.Interfaces
{
    public interface IScrapingService
    {
        /// <summary>
        /// Scrapes the HTML from search results.
        /// </summary>
        /// <param name="searchPhrase">The search phrase.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The page html as a string.</returns>
        public Task<string> ScrapeContent(string searchPhrase, CancellationToken cancellationToken);
    }
}
