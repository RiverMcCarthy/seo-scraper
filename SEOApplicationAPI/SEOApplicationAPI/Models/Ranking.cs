namespace SEOApplicationAPI.Models
{
    public class Ranking
    {
        public string SearchPhrase { get; set; } = null!;

        public string Url { get; set; } = null!;

        public int Rank { get; set; }

        public DateTime SearchedOn { get; set; }
    }
}
