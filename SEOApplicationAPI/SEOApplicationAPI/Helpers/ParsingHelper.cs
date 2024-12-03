using SEOApplicationAPI.Services;
using System.Text.RegularExpressions;

namespace SEOApplicationAPI.Helpers
{
    public class ParsingHelper(
        ILogger<ParsingHelper> logger)
    {
        private readonly ILogger<ParsingHelper> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Retrieve the links from decompressed HTML.
        /// </summary>
        /// <param name="html">The HTML content.</param>
        /// <returns>A list of links which were found.</returns>
        public List<string> ParseLinksFromHtml(string html)
        {
            var links = new List<string>();

            try
            {
                var matches = Regex.Matches(html, @"<a\s+[^>]*jsname\s*=\s*""UWckNb""[^>]*\s+href\s*=\s*""([^\""]+)""[^>]*>", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    var decodedLink = Uri.UnescapeDataString(match.Groups[1].Value);

                    if (Uri.TryCreate(decodedLink, UriKind.Absolute, out _))
                    {
                        links.Add(decodedLink);
                    }
                }
                /*
                while ((index = html.IndexOf("<a", index)) != -1)
                {
                    var tagStart = index;


                    int tagEnd = html.IndexOf(">", tagStart);

                    if (tagEnd == -1) break;

                    var anchorTag = html.Substring(tagStart, tagEnd - tagStart + 1);
                    index = tagEnd + 1;

                    if (anchorTag.Contains("UWckNb"))
                    {
                        var match = Regex.Match(anchorTag, hrefPattern);

                        if (match.Success)
                        {
                            links.Add(match.Groups[1].Value);
                        }
                        var hrefStart = anchorTag.IndexOf("href=\"http") + 10;
                        var hrefEnd = anchorTag.IndexOf(".co") + 3;
                        if (hrefEnd == -1) continue;

                        var link = anchorTag.Substring(hrefStart, hrefEnd - hrefStart);
                        links.Add(link);
                    }
                }*/
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse HTML");
            }

            return links;
        }
    }
}
