namespace TuitionManagementSystem.Infrastructure.DataAccess;

using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Account> Account { get; set; }
}
