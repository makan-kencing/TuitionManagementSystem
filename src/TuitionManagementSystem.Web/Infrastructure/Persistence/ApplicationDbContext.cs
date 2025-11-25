namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Linq.Expressions;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models.User;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> User { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
        this.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;

    // https://stackoverflow.com/questions/37932339/how-can-i-implement-soft-deletes-with-entity-framework-core-aka-ef7
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureSoftDeleteFilter(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureSoftDeleteFilter(ModelBuilder builder)
    {
        foreach (var softDeletableTypeBuilder in builder.Model.GetEntityTypes()
                     .Where(x => typeof(ISoftDeletable).IsAssignableFrom(x.ClrType)))
        {
            var parameter = Expression.Parameter(softDeletableTypeBuilder.ClrType, "p");

            softDeletableTypeBuilder.SetQueryFilter(
                Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt)),
                        Expression.Constant(null)),
                    parameter)
            );
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        foreach (var entry in this.ChangeTracker.Entries<ISoftDeletable>())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                    // Override removal. Unchanged is better than Modified, because the latter flags ALL properties for update.
                    // With Unchanged, the change tracker will pick up on the freshly changed properties and save them.
                    entry.State = EntityState.Unchanged;
                    entry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
