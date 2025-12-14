namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Models.User;
using Services.Auth.Constants;
using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task InitializeAsync(
        DbContext db,
        CancellationToken cancellationToken = default)
    {
        var random = new Random();

        // // -----------------------------
        // // ATTENDANCE CODES
        // // -----------------------------
        // if (!await db.Set<AttendanceCode>().AnyAsync(cancellationToken))
        // {
        //     var codes = Enumerable.Range(0, 1_000_000)
        //         .OrderBy(_ => random.NextDouble())
        //         .Select((v, index) => new AttendanceCode
        //         {
        //             Id = index + 1,
        //             Code = v.ToString("D6", CultureInfo.InvariantCulture)
        //         });
        //
        //     await db.Set<AttendanceCode>().AddRangeAsync(codes, cancellationToken);
        //     await db.SaveChangesAsync(cancellationToken);
        // }

        // STUDENTS + ACCOUNTS
        var passwordHasher = new PasswordHasher<Account>();
        var plainPassword = "123456";

        for (int i = 1; i <= 5; i++)
        {
            var username = $"student{i}";

            var exists = await db.Set<Account>()
                .AnyAsync(a => a.Username == username, cancellationToken);
            if (exists) continue;

            var account = new Account
            {
                Username = username,
                HashedPassword = passwordHasher.HashPassword(null, plainPassword),
                Email = $"student{i}@example.com",
                AccessRole = AccessRoles.User
            };

            var student = new Student { Account = account };

            await db.Set<Student>().AddAsync(student, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);

        // SUBJECTS
        var subjectNames = new[] { "Mathematics", "Science", "English" };
        foreach (var name in subjectNames)
        {
            var exists = await db.Set<Subject>().AnyAsync(s => s.Name == name, cancellationToken);
            if (!exists)
            {
                await db.Set<Subject>().AddAsync(new Subject { Name = name }, cancellationToken);
            }
        }
        await db.SaveChangesAsync(cancellationToken);

        // CLASSROOMS
        var classroomLocations = new[] { "Room A", "Room B", "Room C" };
        foreach (var loc in classroomLocations)
        {
            var exists = await db.Set<Classroom>().AnyAsync(c => c.Location == loc, cancellationToken);
            if (!exists)
            {
                await db.Set<Classroom>().AddAsync(new Classroom { Location = loc }, cancellationToken);
            }
        }
        await db.SaveChangesAsync(cancellationToken);

        // COURSES
        var subjects = await db.Set<Subject>().ToListAsync(cancellationToken);
        var classrooms = await db.Set<Classroom>().ToListAsync(cancellationToken);

        var courseData = new[]
        {
            new { Name = "Basic Algebra", Subject = subjects[0], Classroom = classrooms[0] },
            new { Name = "General Science", Subject = subjects[1], Classroom = classrooms[1] },
            new { Name = "English Grammar", Subject = subjects[2], Classroom = classrooms[2] }
        };

        foreach (var c in courseData)
        {
            var exists = await db.Set<Course>().AnyAsync(x => x.Name == c.Name, cancellationToken);
            if (!exists)
            {
                await db.Set<Course>().AddAsync(new Course
                {
                    Name = c.Name,
                    Subject = c.Subject,
                    PreferredClassroom = c.Classroom
                }, cancellationToken);
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        // ENROLLMENTS
        var students = await db.Set<Student>().ToListAsync(cancellationToken);
        var courses = await db.Set<Course>().ToListAsync(cancellationToken);

        foreach (var student in students)
        {
            var hasEnrollment = await db.Set<Enrollment>()
                .AnyAsync(e => e.Student.Id == student.Id, cancellationToken);
            if (hasEnrollment) continue;

            var course = courses[random.Next(courses.Count)];

            await db.Set<Enrollment>().AddAsync(new Enrollment
            {
                Student = student,
                Course = course,
                EnrolledAt = DateTime.UtcNow.AddDays(-random.Next(1, 30))
            }, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
