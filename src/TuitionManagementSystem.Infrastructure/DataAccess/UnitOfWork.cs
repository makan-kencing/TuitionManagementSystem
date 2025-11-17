namespace TuitionManagementSystem.Infrastructure.DataAccess;

using Application.Services;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IDisposable
{
    private bool disposed;

    private void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            context.Dispose();
        }

        this.disposed = true;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<int> Save() => await context.SaveChangesAsync();
}
