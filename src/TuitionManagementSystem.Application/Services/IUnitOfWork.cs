namespace TuitionManagementSystem.Application.Services;

public interface IUnitOfWork
{
    public Task<int> Save();
}
