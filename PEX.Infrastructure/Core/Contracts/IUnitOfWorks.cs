
using Microsoft.EntityFrameworkCore;

using PEX.Domain.Model;
using PEX.Infrastructure.Database;

namespace PEX.Infrastructure.Core.Contracts;
public interface IUnitOfWorks : IDisposable
{
    ApplicationDbContext DbContext { get; }
    DbSet<T> GetDbSet<T, TKey>() where T : BaseEntity<TKey>;
    int SaveChanges();
    Task<int> SaveChangesAsync();
}
