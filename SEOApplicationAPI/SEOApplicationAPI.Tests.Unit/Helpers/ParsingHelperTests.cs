using NUnit.Framework;
using NSubstitute;
using SEOApplicationAPI.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace SEOApplicationAPI.Tests.Unit.Helpers
{
    [TestFixture]
    public class ParsingHelperTests
    {
        private ParsingHelper _parsingHelper;
        private ILogger<ParsingHelper> _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger<ParsingHelper>>();
            _parsingHelper = new ParsingHelper(_logger);
        }

        [Test]
        public void ParseLinksFromHtml_ValidHtmlWithLinks_ReturnsLinks()
        {
            // Arrange
            var html = "<html><body><a jsname=\"UWckNb\" href=\"https://www.test1.com\"></a><a jsname=\"UWckNb\" href=\"https://www.test2.com\"></a></body></html>";

            // Act
            var result = _parsingHelper.ParseLinksFromHtml(html);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(2));
                Assert.That(result[0], Is.EqualTo("https://www.test1.com"));
                Assert.That(result[1], Is.EqualTo("https://www.test2.com"));
            });
        }

        [Test]
        public void ParseLinksFromHtml_WrongClass_ReturnsEmptyList()
        {
            // Arrange
            var html = "<html><body><a jsname=\"different class\" href=\"https://www.test1.com\"></a></body></html>";

            // Act
            var result = _parsingHelper.ParseLinksFromHtml(html);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ParseLinksFromHtml_InvalidHtml_ReturnsEmptyList()
        {
            // Arrange
            var html = "<html><body><a jsname='UWckNb' href='https://www.test.com'>Link</body></html>";

            // Act
            var result = _parsingHelper.ParseLinksFromHtml(html);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ParseLinksFromHtml_ValidHtmlWithInvalidLinks_ReturnsOnlyValidLinks()
        {
            // Arrange
            var html = "<html><body><a jsname=\"UWckNb\" href=\"https://www.test.com\"></a><a jsname=\"UWckNb\" href=\"invalid_link\"></a></body></html>";

            // Act
            var result = _parsingHelper.ParseLinksFromHtml(html);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0], Is.EqualTo("https://www.test.com"));
            });
        }

        [Test]
        public void ParseLinksFromHtml_EmptyHtml_ReturnsEmptyList()
        {
            // Arrange
            var html = "";

            // Act
            var result = _parsingHelper.ParseLinksFromHtml(html);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
