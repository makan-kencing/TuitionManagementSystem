namespace TuitionManagementSystem.Domain.Specifications;

using Entities.Account;

public class AccountSpecification : BaseSpecification<Account>
{
    public AccountSpecification ByEmail(string email)
    {
        this.Wheres.Add(a => a.Email == email);
        return this;
    }
}
