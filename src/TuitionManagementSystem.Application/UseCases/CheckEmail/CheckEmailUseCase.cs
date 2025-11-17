namespace TuitionManagementSystem.Application.UseCases.CheckEmail;

using Domain;
using Domain.Entities.Account;
using Domain.Specifications;

public sealed class CheckEmailUseCase(IRepository<Account> repository) : ICheckEmailUseCase
{
    private IOutputPort outputPort = new CheckEmailPresenter();

    public Task Execute(string email) => this.CheckEmailInternal(email);

    public void SetOutputPort(IOutputPort outputPort) => this.outputPort = outputPort;

    private async Task CheckEmailInternal(string email)
    {
        var specs = new AccountSpecification().ByEmail(email);

        this.outputPort.Ok(await repository.ExistsAsync(specs, CancellationToken.None));
    }
}
