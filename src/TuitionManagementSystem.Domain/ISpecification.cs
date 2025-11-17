namespace TuitionManagementSystem.Domain;

using System.Linq.Expressions;

public interface ISpecification<TEntity, TProjection>
{
    public IList<Expression<Func<TEntity, bool>>> Wheres { get; }

    public IList<Expression<Func<TEntity, object>>> Includes { get; }

    public IList<Expression<Func<TProjection, object>>> OrderBys { get; }

    public Expression<Func<TEntity, TProjection>>? Select { get; }

    public ushort? Take { get; }

    public uint? Skip { get; }
}
