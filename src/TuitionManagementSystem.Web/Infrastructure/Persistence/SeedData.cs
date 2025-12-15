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
        var random = new Random(12345);
        var passwordHasher = new PasswordHasher<Account>();
        var plainPassword = "123456";

        // // ATTENDANCE CODES
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
        var studentUsernames = Enumerable.Range(1, 5).Select(i => $"student{i}").ToList();
        var existingAccounts = await db.Set<Account>()
            .Where(a => studentUsernames.Contains(a.Username))
            .Select(a => a.Username)
            .ToListAsync(cancellationToken);

        var newStudents = studentUsernames
            .Except(existingAccounts)
            .Select(u => new Student
            {
                Account = new Account
                {
                    Username = u,
                    HashedPassword = passwordHasher.HashPassword(null, plainPassword),
                    Email = $"{u}@example.com",
                    AccessRole = AccessRoles.User
                }
            })
            .ToList();

        if (newStudents.Any())
        {
            await db.Set<Student>().AddRangeAsync(newStudents, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        // SUBJECTS
        var subjectNames = new[] { "Mathematics", "Science", "English" };
        var existingSubjects = await db.Set<Subject>()
            .Where(s => subjectNames.Contains(s.Name))
            .Select(s => s.Name)
            .ToListAsync(cancellationToken);

        var newSubjects = subjectNames
            .Except(existingSubjects)
            .Select(n => new Subject { Name = n })
            .ToList();

        if (newSubjects.Any())
        {
            await db.Set<Subject>().AddRangeAsync(newSubjects, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        // CLASSROOMS
        var classroomData = new[]
        {
            new { Location = "Room A", MaxCapacity = 30 },
            new { Location = "Room B", MaxCapacity = 25 },
            new { Location = "Room C", MaxCapacity = 20 }
        };

        var existingClassrooms = await db.Set<Classroom>()
            .Where(c => classroomData.Select(d => d.Location).Contains(c.Location))
            .Select(c => c.Location)
            .ToListAsync(cancellationToken);

        var newClassrooms = classroomData
            .Where(c => !existingClassrooms.Contains(c.Location))
            .Select(c => new Classroom
            {
                Location = c.Location,
                MaxCapacity = c.MaxCapacity
            })
            .ToList();

        if (newClassrooms.Any())
        {
            await db.Set<Classroom>().AddRangeAsync(newClassrooms, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        // COURSES
        var subjects = await db.Set<Subject>().ToListAsync(cancellationToken);
        var classrooms = await db.Set<Classroom>().ToListAsync(cancellationToken);

        var courseData = new[]
        {
            new { Name = "Basic Algebra", Subject = subjects[0], Classroom = classrooms[0] },
            new { Name = "General Science", Subject = subjects[1], Classroom = classrooms[1] },
            new { Name = "English Grammar", Subject = subjects[2], Classroom = classrooms[2] }
        };

        var existingCourses = await db.Set<Course>()
            .Where(c => courseData.Select(d => d.Name).Contains(c.Name))
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);

        var newCourses = courseData
            .Where(c => !existingCourses.Contains(c.Name))
            .Select(c => new Course
            {
                Name = c.Name,
                Subject = c.Subject,
                PreferredClassroom = c.Classroom
            })
            .ToList();

        if (newCourses.Any())
        {
            await db.Set<Course>().AddRangeAsync(newCourses, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        // ENROLLMENTS
        var studentsList = await db.Set<Student>().ToListAsync(cancellationToken);
        var coursesList = await db.Set<Course>().ToListAsync(cancellationToken);

        var newEnrollments = studentsList
            .Where(s => !db.Set<Enrollment>().Any(e => e.Student.Id == s.Id))
            .Select(s => new Enrollment
            {
                Student = s,
                Course = coursesList[random.Next(coursesList.Count)],
                EnrolledAt = DateTime.UtcNow.AddDays(-random.Next(1, 30))
            })
            .ToList();

        if (newEnrollments.Any())
        {
            await db.Set<Enrollment>().AddRangeAsync(newEnrollments, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
