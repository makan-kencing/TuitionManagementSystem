using Microsoft.EntityFrameworkCore;

namespace TuitionManagementSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
}