
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using PEX.Domain.Model;
using PEX.Infrastructure.Core.Contracts;
using PEX.Infrastructure.Helper;

namespace PEX.Infrastructure.Core;
public abstract class BaseEFRepository<T, TKey> : IBaseRepository<T, TKey>
            where T : BaseEntity<TKey>
{
    //
    private readonly DbSet<T> _dbSet;
    private readonly IUnitOfWorks _unitOfWorks;
    private readonly IServiceProvider _serviceProvider;
    //
    public abstract IQueryable<T> GetFullDbSet();

    //
    #region Constructor ...
    protected BaseEFRepository(IUnitOfWorks unitOfWorks, IServiceProvider serviceProvider)
    {
        //
        _unitOfWorks = unitOfWorks;
        _dbSet = _unitOfWorks.GetDbSet<T, TKey>();
        _serviceProvider = serviceProvider;
    }
    #endregion

    //
    #region Add ...
    public async Task<T> AddAsync(T item, CancellationToken cancellationToken, bool saveChanges = true)
    {
        //
        var entry = await _dbSet.AddAsync(item, cancellationToken);

        //
        if (saveChanges)
        {
            await SaveChangesAsync();
        }

        //
        var result = entry.Entity;

        //
        return result;
    }

    public async Task<T> AddOrUpdateAsync(T item, CancellationToken cancellationToken, bool saveChanges = true)
    {
        //
        TKey? key = GetKey(item);
        if (key is null)
        {
            throw new ArgumentNullException(paramName: nameof(item));
        }
        var isExists = await IsExistsAsync(key);
        if (!isExists)
        {
            return await AddAsync(item, cancellationToken, saveChanges);
        }

        //
        var entry = _dbSet.Attach(item);
        entry.State = EntityState.Modified;

        //
        if (saveChanges)
        {
            await SaveChangesAsync();
        }

        //
        return entry.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken, bool saveChanges = true)
    {
        //
        await _dbSet.AddRangeAsync(items, cancellationToken);

        //
        if (saveChanges)
        {
            await SaveChangesAsync();
        }
    }
    #endregion

    //
    #region Remove ...
    public async Task<T> RemoveAsync(TKey id, bool saveChanges = true)
    {
        //
        var isExists = await IsExistsAsync(id);
        if (!isExists)
        {
            throw new InvalidOperationException($"No Entity Found Using {id}");
        }

        //
        var item = await GetAsync(id);
        var entry = _dbSet.Remove(item);

        //
        if (saveChanges)
        {
            await SaveChangesAsync();
        }

        //
        var result = entry.Entity;

        //
        return result;
    }

    public async Task<T> RemoveAsync(T item, bool saveChanges = true)
    {
        //
        TKey? key = GetKey(item);
        if (key is null)
        {
            throw new ArgumentNullException(paramName: nameof(item));
        }
        var isExists = await IsExistsAsync(key);
        if (!isExists)
        {
            throw new InvalidOperationException($"No Entity Found");
        }

        //
        var entry = _dbSet.Remove(item);

        //
        if (saveChanges)
        {
            await SaveChangesAsync();
        }

        //
        var result = entry.Entity;

        //
        return result;
    }

    public async Task RemoveRangeAsync(IEnumerable<T> items, bool saveChanges = true)
    {
        //
        _dbSet.RemoveRange(items);

        //
        if (saveChanges)
        {
            await SaveChangesAsync();
        }
    }
    #endregion

    //
    #region Retrieve ...
    public async Task<T> GetAsync(TKey id, bool containsDetail = false)
    {
        //
        var result = await FindOneAsync(i =>
           string.Equals(GetKey(i)!.ToString(), id!.ToString(), StringComparison.Ordinal),
            containsDetail: containsDetail
        );

        //
        return result;
    }

    public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool containsDetail = false)
    {
        return Task.Run(() => GetDbSet(containsDetail: containsDetail).AsEnumerable(), cancellationToken);
    }

    public async Task<T> FindOneAsync(Expression<Func<T, bool>> expression, bool containsDetail = false)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
        //
        // Generate Where Function ...
        var whereFunc = expression.Compile();

        //
        // Get Enumerable ...
        var enumerator = GetDbSet(
                containsDetail: containsDetail
            )
            .AsAsyncEnumerable();

        //
        T? result = null;
        await
        foreach (var entity in enumerator)
        {
            //
            var isApproved = whereFunc(entity);
            if (isApproved)
            {
                //
                result = entity;
                break;
            }
        }

        //
        if (result is null)
        {
            throw new InvalidOperationException();
        }
        return result;
    }

    public async Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> expression, bool containsDetail = false)
    {

        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
        //
        // Generate Where Function ...
        var whereFunc = expression.Compile();

        //
        // Get Enumerable ...
        var enumerator = GetDbSet(
                containsDetail: containsDetail
            )
            .AsAsyncEnumerable();

        //
        var result = new List<T>();
        await
        foreach (var entity in enumerator)
        {
            //
            var isApproved = whereFunc(entity);
            if (isApproved)
            {
                result.Add(entity);
            }
        }

        //
        return result.AsEnumerable();
    }


    #endregion

    //
    #region Update ...
    public async Task<T> UpdateAsync(TKey id, T item, bool saveChanges = true)
    {
        //
        // Get Exists Entity ...
        var existsEntity = await GetAsync(id, containsDetail: true);



        //
        // Update Data ...
        existsEntity.UpdateData(item);
        var entry = _dbSet.Attach(existsEntity);
        entry.State = EntityState.Modified;

        //
        if (saveChanges)
        {
            await SaveChangesAsync();
        }

        //
        var result = entry.Entity;

        //
        return result;
    }

    public async Task<bool> UpdateRangeAsync(IEnumerable<T> items, bool saveChanges = true)
    {
        //
        items
            .ToList()
            .ForEach(i =>
            {
                var entry = _dbSet.Attach(i);
                entry.State = EntityState.Modified;
            });

        //
        var res = 0;
        if (saveChanges)
        {
            res = await SaveChangesAsync();
        }

        //
        return res > 0;
    }
    #endregion

    //
    #region Count ...
    public Task<int> CountAsync()
    {
        return GetDbSet()
            .CountAsync();
    }

    public async Task<int> PagesCountAsync(int pageSize)
    {
        //
        int count = await CountAsync();
        int pagesCount = count / pageSize;

        //
        if (count % pageSize > 0)
        {
            pagesCount++;
        }

        //
        return pagesCount;
    }
    #endregion

    //
    #region Exists ...
    public async Task<bool> IsExistsAsync(TKey id)
    {

        //
        var result = await FindOneAsync(i =>
           string.Equals(GetKey(i)!.ToString(), id!.ToString(), StringComparison.Ordinal)
        );

        //
        return !result.IsNull();
    }
    #endregion

    //
    #region Unit Of Work ...
    public async Task<int> SaveChangesAsync()
    {
        return await _unitOfWorks.SaveChangesAsync();
    }
    #endregion

    //
    #region Detach ...
    public void Detach(T item)
    {
        //
        if (item.IsNull())
        {
            return;
        }

        //
        Entry(item).State = EntityState.Detached;
    }

    public void Detach(IEnumerable<T> items)
    {
        //
        if (items.IsNull() || !items.HasChild())
        {
            return;
        }

        //
        items
            .ToList()
            .ForEach(item =>
            {
                Detach(item);
            });
    }

    #endregion

    //
    #region Others ...
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _unitOfWorks.Dispose();
        }
    }

    public TKey? GetKey(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(paramName: nameof(item));
        }
        //
        var props = item.GetType().GetProperties();
        var keyProp = props.FirstOrDefault(p => string.Equals(p.Name, "Id", StringComparison.Ordinal));

        //
        var keyString = string.Empty;
        if (keyProp!.IsNull())
        {
            keyString = string.Empty;
        }
        else
        {
            keyString = keyProp!.GetValue(item)?.ToString();
        }

        //
        if (keyString!.IsNullOrEmpty())
        {
            return default;
        }

        //
        return keyString!.FromJSON<TKey>();
    }

    public string GetPropValues(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(paramName: nameof(item));
        }
        //
        var props = item.GetType().GetProperties();
        var vals = props.Select(p => p.GetValue(p.Name));

        //
        return vals.ToJSON();
    }

    public EntityEntry<T> Entry(T item)
    {
        return _unitOfWorks.DbContext.Entry(item);
    }

    public IQueryable<T> GetDbSet(bool containsDetail = false)
    {
        //
        IQueryable<T> result = !containsDetail ? _dbSet : GetFullDbSet();

        //
        return result;
    }


    #endregion
}