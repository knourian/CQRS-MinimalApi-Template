
using Microsoft.EntityFrameworkCore;

using PEX.Domain.Model;
using PEX.Infrastructure.Core.Contracts;
using PEX.Infrastructure.Database;

namespace PEX.Infrastructure.Core;
public class UnitOfWorks : IUnitOfWorks
{
    public ApplicationDbContext DbContext { get; }

    public UnitOfWorks(ApplicationDbContext context)
    {
        DbContext = context;
    }

    public DbSet<T> GetDbSet<T, TKey>()
    where T : BaseEntity<TKey>
    {
        return DbContext.Set<T>();
    }

    /// <summary>
    /// Save Changes
    /// </summary>        
    public int SaveChanges()
    {
        return DbContext.SaveChanges();
    }

    /// <summary>
    /// Save Changes Async
    /// </summary>
    /// <returns></returns>
    public async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContext.Dispose();
        }
    }
}
