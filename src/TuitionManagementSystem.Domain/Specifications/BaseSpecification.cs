namespace TuitionManagementSystem.Domain.Specifications;

using System.Linq.Expressions;

public abstract class BaseSpecification<T> : BaseSpecification<T, T> where T : class;

public abstract class BaseSpecification<TEntity, TProjection> : ISpecification<TEntity, TProjection> where TEntity : class
{
    public IList<Expression<Func<TEntity, bool>>> Wheres { get; protected set; } = new List<Expression<Func<TEntity, bool>>>();

    public IList<Expression<Func<TEntity, object>>> Includes { get; protected set; } = new List<Expression<Func<TEntity, object>>>();

    public IList<Expression<Func<TProjection, object>>> OrderBys { get; protected set; } = new List<Expression<Func<TProjection, object>>>();

    public Expression<Func<TEntity, TProjection>>? Select { get; protected set; }

    public ushort? Take { get; protected set; }

    public uint? Skip { get; protected set; }
}
