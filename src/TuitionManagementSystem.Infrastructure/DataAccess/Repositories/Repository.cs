namespace TuitionManagementSystem.Infrastructure.DataAccess.Repositories;

using Domain;
using Microsoft.EntityFrameworkCore;

public class Repository<T>(ApplicationDbContext db) : IRepository<T>
    where T : class
{
    public async Task<T> CreateAsync(T record, CancellationToken cancellationToken = default) =>
        (await db.Set<T>().AddAsync(record, cancellationToken)).Entity;

    public async Task<bool> ExistsAsync(ISpecification<T, T> specification,
        CancellationToken cancellationToken = default) =>
        (await this.GetAsync(specification, cancellationToken)) != null;

    public async Task<TProjection?> GetAsync<TProjection>(ISpecification<T, TProjection> specification,
        CancellationToken cancellationToken = default)
    {
        var query = this.ProcessQuery(specification);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TProjection>> FetchAsync<TProjection>(
        ISpecification<T, TProjection> specification,
        CancellationToken cancellationToken = default)
    {
        var query = this.ProcessQuery(specification);
        if (specification.Skip.HasValue)
        {
            query = query.Skip((int)specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(T record, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await db.SaveChangesAsync(cancellationToken);

    private IQueryable<TProjection> ProcessQuery<TProjection>(ISpecification<T, TProjection> specification)
    {
        IQueryable<TProjection> projectedQuery;
        var query = db.Set<T>().AsQueryable();

        if (specification.Wheres.Any())
        {
            foreach (var whereExpression in specification.Wheres)
            {
                query = query.Where(whereExpression);
            }
        }

        if (specification.Includes.Any())
        {
            foreach (var includeExpression in specification.Includes)
            {
                query = query.Include(includeExpression);
            }
        }

        if (specification.Select != null)
        {
            projectedQuery = query.Select(specification.Select);
        }
        else
        {
            projectedQuery = (IQueryable<TProjection>)query;
        }

        if (specification.OrderBys.Any())
        {
            for (var i = 0; i < specification.OrderBys.Count; i++)
            {
                var orderByExpression = specification.OrderBys.ElementAt(i);
                if (i == 0)
                {
                    projectedQuery = projectedQuery.OrderBy(orderByExpression);
                }
                else
                {
                    projectedQuery = ((IOrderedQueryable<TProjection>)projectedQuery).ThenBy(orderByExpression);
                }
            }
        }

        return projectedQuery;
    }
}
