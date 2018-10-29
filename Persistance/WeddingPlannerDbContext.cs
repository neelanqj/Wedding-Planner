using Microsoft.EntityFrameworkCore;
using Wedding_Planner.Models;

namespace Wedding_Planner.Persistance
{
    public class WeddingPlannerDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<UserWedding_Xrf> UserWedding_Xrf { get; set; }
        
        public WeddingPlannerDbContext(DbContextOptions<WeddingPlannerDbContext> options)
            : base(options)
        {
            
        }
    }
}