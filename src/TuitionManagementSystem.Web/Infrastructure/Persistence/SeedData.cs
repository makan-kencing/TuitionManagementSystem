namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public static class SeedData
{
    public static async Task InitializeAsync(
        DbContext db,
        CancellationToken cancellationToken = default)
    {
        var random = new Random();

        if (!await db.Set<AttendanceCode>()
                .AnyAsync(cancellationToken))
        {
            var codes = Enumerable.Range(0, 1_000_000)
                .OrderBy(_ => random.NextDouble())
                .Select((v, index) => new AttendanceCode
                {
                    Id = index + 1,
                    Code = v.ToString("D6", CultureInfo.InvariantCulture)
                });

            await db.Set<AttendanceCode>()
                .AddRangeAsync(codes, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

    }
}
