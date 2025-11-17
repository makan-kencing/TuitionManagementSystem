namespace TuitionManagementSystem.Infrastructure.EntityFrameworkDataAccess;

using Application.Services;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IDisposable
{
    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (this._disposed)
        {
            return;
        }

        if (disposing)
        {
            context.Dispose();
        }

        this._disposed = true;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<int> Save() => await context.SaveChangesAsync();
}
