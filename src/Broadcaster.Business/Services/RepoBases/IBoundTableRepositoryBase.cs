using System;

namespace Broadcaster.Business.Services.RepoBases;

public interface IBoundTableRepositoryBase<T>
{
    Task<IEnumerable<T>> CachedGetAsync();

    T? GetById(int id);

    Task<T?> GetByIdAsync(int id);

    T? GetByKey(string key);

    Task<T?> GetByKeyAsync(string key);

    IEnumerable<T> Get();

    IEnumerable<T> Get(object? whereConditions, object? whereNotConditions = null);

    Task<IEnumerable<T>> GetAsync();

    Task<IEnumerable<T>> GetAsync(object? whereConditions, object? whereNotConditions = null);

    IEnumerable<T> GetListPaged(int pageNumber, int rowsPerPage, string orderBy, object? whereConditions = null, object? whereNotConditions = null);

    Task<IEnumerable<T>> GetListPagedAsync(int pageNumber, int rowsPerPage, string orderBy, object? whereConditions = null, object? whereNotConditions = null);

    int RecordCount(object? whereConditions = null, object? whereNotConditions = null);

    Task<int> RecordCountAsync(object? whereConditions = null, object? whereNotConditions = null);

    Task<T> SaveAsync(T entity);

    T Insert(T entity);

    Task<T> InsertAsync(T entity);

    int Update(T entity);

    Task<int> UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task DeleteByIdAsync(int id);

    Task DeleteByKeyAsync(string key);
}
