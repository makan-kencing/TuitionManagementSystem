namespace TuitionManagementSystem.Infrastructure.DataAccess;

using Microsoft.EntityFrameworkCore;
using Entities;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}
