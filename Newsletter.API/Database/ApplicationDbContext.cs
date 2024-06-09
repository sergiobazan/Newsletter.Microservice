using Microsoft.EntityFrameworkCore;
using Newsletter.API.Entities;

namespace Newsletter.API.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt)
        : base(opt)
    {
        
    }

    public DbSet<Article> Articles { get; set; }
}
