namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Linq.Expressions;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models.Class;
using Models.Class.Announcement;
using Models.Payment;
using Models.User;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<ScheduleRecurrencePattern> ScheduleRecurrencePatterns { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
        this.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankPaymentMethod>();
        modelBuilder.Entity<CardPaymentMethod>();
        modelBuilder.Entity<GenericPaymentMethod>();

        ConfigureSoftDeleteFilter(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    // https://stackoverflow.com/questions/37932339/how-can-i-implement-soft-deletes-with-entity-framework-core-aka-ef7
    private static void ConfigureSoftDeleteFilter(ModelBuilder builder)
    {
        foreach (var softDeletableTypeBuilder in builder.Model.GetEntityTypes()
                     .Where(x => typeof(ISoftDeletable).IsAssignableFrom(x.ClrType)
                                 && x.BaseType == null))
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
