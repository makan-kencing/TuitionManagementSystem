using Microsoft.EntityFrameworkCore;

namespace TuitionManagementSystem.Infrastructure.EntityFrameworkDataAccess;

using Entities;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}
