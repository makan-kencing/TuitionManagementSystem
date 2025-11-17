namespace TuitionManagementSystem.Domain;

public interface IRepository<T> where T : class
{
    public Task<TProjection?> GetAsync<TProjection>(ISpecification<T, TProjection> specification, CancellationToken cancellationToken);

    public Task<IEnumerable<TProjection>> FetchAsync<TProjection>(ISpecification<T, TProjection> specification, CancellationToken cancellationToken);

    public Task<T> CreateAsync(T record, CancellationToken cancellationToken);

    public Task UpdateAsync(T record, CancellationToken cancellationToken);

    public Task<bool> ExistsAsync(ISpecification<T, T> specification, CancellationToken cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken);
}
