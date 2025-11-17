namespace TuitionManagementSystem.Infrastructure.EntityFrameworkDataAccess.Repositories;

using Entities;

public sealed class UserRepository(ApplicationDbContext context)
{
    public async Task Add(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}
