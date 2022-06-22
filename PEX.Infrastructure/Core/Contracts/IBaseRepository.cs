using PEX.Domain.Model;

namespace PEX.Infrastructure.Core.Contracts;
/// <summary>
/// a Base Repository Interface for Manipulate 
/// an Entity in DataBase 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBaseRepository<T, TKey> : IDisposable
where T : BaseEntity<TKey>
{
    //
    #region Add ...
    Task<T> AddAsync(T item, CancellationToken cancellationToken, bool saveChanges = true);
    Task<T> AddOrUpdateAsync(T item, CancellationToken cancellationToken, bool saveChanges = true);
    Task AddRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken, bool saveChanges = true);
    #endregion

    //
    #region Remove ...
    Task<T> RemoveAsync(TKey id, bool saveChanges = true);
    Task<T> RemoveAsync(T item, bool saveChanges = true);
    Task RemoveRangeAsync(IEnumerable<T> items, bool saveChanges = true);
    #endregion

    //
    #region Retrieve ...
    Task<T> GetAsync(TKey id, bool containsDetail = false);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool containsDetail = false);
    Task<T> FindOneAsync(Expression<Func<T, bool>> expression, bool containsDetail = false);
    Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> expression, bool containsDetail = false);
    #endregion

    //
    #region Update ...
    Task<T> UpdateAsync(
        TKey id,
        T item,
        bool saveChanges = true
    );
    Task<bool> UpdateRangeAsync(
        IEnumerable<T> items,
        bool saveChanges = true
    );
    #endregion

    //
    #region Count ...
    Task<int> CountAsync();
    Task<int> PagesCountAsync(
        int pageSize
    );
    #endregion

    //
    #region Exists ...
    Task<bool> IsExistsAsync(TKey id);
    #endregion

    //
    #region Unit Of Work ...
    Task<int> SaveChangesAsync();
    #endregion

    //
    #region Detach ...
    void Detach(T item);

    void Detach(IEnumerable<T> items);
    #endregion


}