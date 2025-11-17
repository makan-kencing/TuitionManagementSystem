namespace TuitionManagementSystem.Application.UseCases.CheckEmail;

using System.Runtime.CompilerServices;

public sealed class CheckEmailPresenter : IOutputPort
{
    public bool Found { get; private set; }
    public void Ok(bool found) => this.Found = found;
}
