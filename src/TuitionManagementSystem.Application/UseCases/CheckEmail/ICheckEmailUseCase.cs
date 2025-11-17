namespace TuitionManagementSystem.Application.UseCases.CheckEmail;

public interface ICheckEmailUseCase
{
    public Task Execute(string email);

    public void SetOutputPort(IOutputPort outputPort);
}
