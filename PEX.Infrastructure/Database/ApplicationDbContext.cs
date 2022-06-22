
using Microsoft.EntityFrameworkCore;

using PEX.Domain.Model;


namespace PEX.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }


    public DbSet<Author> Authors { get; set; } = default!;
}
