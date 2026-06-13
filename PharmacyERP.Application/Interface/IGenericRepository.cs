public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);

    Task<List<T>> GetAllAsync(bool asNoTracking = true);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    IQueryable<T> Query();

    Task SaveChangesAsync();
}