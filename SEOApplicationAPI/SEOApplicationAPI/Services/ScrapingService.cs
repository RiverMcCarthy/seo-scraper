using SEOApplicationAPI.Interfaces;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace SEOApplicationAPI.Services
{
    public class ScrapingService : IScrapingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScrapingService> _logger;
        private readonly IConfiguration _configuration;

        public ScrapingService(
            HttpClient httpClient,
            ILogger<ScrapingService> logger,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Set the request headers to mimick browser request.
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en-GB,en;q=0.9");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://www.google.com/");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        public async Task<string> ScrapeContent(string searchPhrase, CancellationToken cancellationToken)
        {
            var numResults = _configuration.GetValue<int>("AppSettings:NumberOfResults");
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            string url = $"{baseUrl}/search?num={numResults}&q={Uri.EscapeDataString(searchPhrase)}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Request succeeded with status code: {StatusCode}", response.StatusCode);

                    var contentEncoding = response.Content.Headers.ContentEncoding.ToString();
                    var rawBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

                    // Decompress response
                    string htmlContent = contentEncoding switch
                    {
                        "gzip" => DecompressGzip(rawBytes),
                        "deflate" => DecompressDeflate(rawBytes),
                        "br" => DecompressBrotli(rawBytes),
                        _ => Encoding.UTF8.GetString(rawBytes)
                    };

                    return htmlContent;
                }
                else
                {
                    _logger.LogWarning("Request failed with status code: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"Request to {url} failed with status code {response.StatusCode}.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HttpRequestException occurred.");
                throw;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, "Search was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching search results.");
                throw;
            }
            
        }

        private string DecompressGzip(byte[] rawBytes)
        {
            using (var compressedStream = new MemoryStream(rawBytes))
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream))
            {
                return reader.ReadToEnd();
            }
        }

        private string DecompressDeflate(byte[] rawBytes)
        {
            using (var compressedStream = new MemoryStream(rawBytes))
            using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(deflateStream))
            {
                return reader.ReadToEnd();
            }
        }

        private string DecompressBrotli(byte[] rawBytes)
        {
            using (var compressedStream = new MemoryStream(rawBytes))
            using (var brotliStream = new BrotliStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(brotliStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
        
}
