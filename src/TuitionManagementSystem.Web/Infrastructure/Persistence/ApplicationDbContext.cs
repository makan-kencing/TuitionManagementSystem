namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Account> Account { get; set; }
}
