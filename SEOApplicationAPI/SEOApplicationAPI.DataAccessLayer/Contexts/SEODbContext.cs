using Microsoft.EntityFrameworkCore;
using SEOApplicationAPI.DataAccessLayer.Models;

namespace SEOApplicationAPI.DataAccessLayer.Contexts
{
    public class SEODbContext(DbContextOptions<SEODbContext> options) : DbContext(options)
    {
        public DbSet<RankingDbo> Rankings { get; set; }
    }
}
