using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEOApplicationAPI.DataAccessLayer.Contexts;
using SEOApplicationAPI.DataAccessLayer.Models;
using SEOApplicationAPI.Helpers;
using SEOApplicationAPI.Interfaces;
using SEOApplicationAPI.Models;
using System;
using System.Net.Http;

namespace SEOApplicationAPI.Services
{
    public class RankingService(
        ILogger<RankingService> logger,
        ILogger<ParsingHelper> parsingLogger,
        SEODbContext seoDbContext,
        IMapper mapper) : IRankingService
    {

        private readonly SEODbContext _context = seoDbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<RankingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ILogger<ParsingHelper> _parsingLogger = parsingLogger ?? throw new ArgumentNullException(nameof(parsingLogger));


        public async Task<List<int>> GetRankingsAsync(string html, string searchPhrase, string targetUrl)
        {
            var parsingHelper = new ParsingHelper(_parsingLogger);

            var links = parsingHelper.ParseLinksFromHtml(html);
            List<int> rankings = [];
            var rank = 1;

            foreach (var link in links)
            {
                if (link.Contains(targetUrl))
                {
                    rankings.Add(rank);
                    _context.Add(
                        new RankingDbo
                        {
                            SearchPhrase = searchPhrase,
                            Url = targetUrl,
                            Rank = rank,
                            SearchedOn = DateTime.UtcNow,
                        });
                }

                rank++;
            }

            // Return "0" if no matching links are found.
            if (rankings.Count == 0)
            {
                rankings.Add(0);
                _context.Add(
                    new RankingDbo
                    {
                        SearchPhrase = searchPhrase,
                        Url = targetUrl,
                        Rank = 0,
                        SearchedOn = DateTime.UtcNow,
                    });
            };

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Failed to update database: {ex!.Message}");
            }

            return rankings;
        }

        public async Task<Rankings> GetAllRankingsAsync()
        {
            Rankings rankingData = null!;

            try
            {
                var rankings = await _context.Rankings.ToListAsync();

                rankingData = _mapper.Map<Rankings>(rankings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update database: {ex!.Message}");
            }

            return rankingData;
        }
    }
}
