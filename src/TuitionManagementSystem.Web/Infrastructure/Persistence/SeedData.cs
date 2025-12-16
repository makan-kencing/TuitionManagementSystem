namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Globalization;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Models.User;
using Services.Auth.Constants;
using Microsoft.AspNetCore.Identity;

public class SeedData
{
    private Random random = new(1);

    private readonly List<Subject> subjects;
    private readonly List<Classroom> classrooms;
    private readonly List<Teacher> teachers;
    private readonly List<Student> students;
    private readonly List<Parent> parents;
    private readonly List<Course> courses;
    private readonly List<Enrollment> enrollments;
    private readonly List<Session> sessions = [];
    private readonly List<Attendance> attendances = [];

    private IEnumerable<User> Users => this.teachers.Cast<User>()
        .Concat(this.students)
        .Concat(this.parents);

    private const string DefaultPassword = "123456";
    private readonly PasswordHasher<object> passwordHasher = new();
    private string HashedDefaultPassword => this.passwordHasher.HashPassword(null!, DefaultPassword);

    private readonly DbContext db;

    public SeedData(DbContext db)
    {
        this.db = db;

        this.subjects =
        [
            new Subject { Name = "Algebra" },
            new Subject { Name = "Science" },
            new Subject { Name = "Advanced English" }
        ];

        this.classrooms =
        [
            new Classroom { Location = "Room A", MaxCapacity = 30 },
            new Classroom { Location = "Room B", MaxCapacity = 25 },
            new Classroom { Location = "Room C", MaxCapacity = 20 }
        ];

        this.courses =
        [
            new Course { Name = "Basic Algebra", Subject = this.subjects[0], PreferredClassroom = this.classrooms[0], Price = 100.00m },
            new Course { Name = "General Science", Subject = this.subjects[1], PreferredClassroom = this.classrooms[1], Price = 120.00m },
            new Course { Name = "English Grammar", Subject = this.subjects[2], PreferredClassroom = this.classrooms[2], Price = 110.00m }
        ];

        this.teachers = Enumerable.Range(1, 3)
            .Select(i => new Teacher
            {
                Account = new Account
                {
                    Username = $"admin{i}",
                    DisplayName = $"Admin Teacher {i}",
                    HashedPassword = this.HashedDefaultPassword,
                    Email = $"admin{i}@example.com",
                    AccessRole = AccessRoles.Administrator
                }
            }).Concat(Enumerable.Range(1, 6)
                .Select(i => new Teacher
                {
                    Account = new Account
                    {
                        Username = $"teacher{i}",
                        DisplayName= $"Teacher {i}",
                        HashedPassword = this.HashedDefaultPassword,
                        Email = $"teacher{i}@example.com",
                        AccessRole = AccessRoles.User
                    }
                }))
            .ToList();

        this.students = Enumerable.Range(1, 20)
            .Select(i => new Student
            {
                Account = new Account
                {
                    Username = $"student{i}",
                    DisplayName = $"Student {i}",
                    HashedPassword = this.HashedDefaultPassword,
                    Email = $"student{i}@example.com",
                    AccessRole = AccessRoles.User
                }
            }).ToList();

        this.parents = Enumerable.Range(1, 8)
            .Select(i => new Parent
            {
                Account = new Account
                {
                    Username = $"parent{i}",
                    DisplayName = $"Parent {i}",
                    HashedPassword = this.HashedDefaultPassword,
                    Email = $"parent{i}@example.com",
                    AccessRole = AccessRoles.User
                }
            }).ToList();

        this.enrollments = this.students
            .Select(s => this.courses.OrderBy(_ => this.random.Next())
                    .Take(this.random.Next(this.courses.Count))
                    .Select(c => new Enrollment
                    {
                        Student = s,
                        Course = c,
                        EnrolledAt = DateTime.UtcNow.AddDays(-this.random.Next(1, 30))
                    })
            )
            .SelectMany(i => i)
            .ToList();

    }

    private async Task SeedSessions(CancellationToken cancellationToken)
    {
        if (await this.db.Set<Session>().AnyAsync(cancellationToken))
            return;

        foreach (var course in this.courses)
        {
            for (int i = 0; i < 5; i++)
            {
                var start = DateTime.UtcNow
                    .Date
                    .AddDays(-14 + i * 2)
                    .AddHours(9);

                this.sessions.Add(new Session
                {
                    Course = course,
                    Classroom = course.PreferredClassroom,
                    StartAt = start,
                    EndAt = start.AddHours(2)
                });
            }
        }

        await this.db.Set<Session>().AddRangeAsync(this.sessions, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAttendances(CancellationToken cancellationToken)
    {
        if (await this.db.Set<Attendance>().AnyAsync(cancellationToken))
            return;

        // 用 Id，不用 reference
        var sessionsByCourseId = this.sessions
            .GroupBy(s => s.Course.Id)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var enrollment in this.enrollments)
        {
            // 找这个学生 enroll 的 course 的 sessions
            if (!sessionsByCourseId.TryGetValue(enrollment.Course.Id, out var courseSessions))
                continue;

            foreach (var session in courseSessions)
            {
                // 如果你要「每个 account 都有 attendance」
                // 就不要 random
                this.attendances.Add(new Attendance
                {
                    Student = enrollment.Student,
                    Session = session,
                    TakenOn = session.StartAt.AddMinutes(this.random.Next(0, 10))
                });
            }
        }

        await this.db.Set<Attendance>().AddRangeAsync(this.attendances, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);
    }




    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await this.SeedAttendanceCodes(cancellationToken);
        await this.Seed(this.subjects, cancellationToken);
        await this.Seed(this.classrooms, cancellationToken);
        await this.Seed(this.courses, cancellationToken);
        await this.Seed(this.Users, cancellationToken);
        await this.Seed(this.enrollments, cancellationToken);

        await this.SeedSessions(cancellationToken);
        await this.SeedAttendances(cancellationToken);

    }

    private async Task Seed<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
    {
        this.random = new Random(1);

        if (!await this.db.Set<TEntity>().AnyAsync(cancellationToken))
        {
            await this.db.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            await this.db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task BulkSeed<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
    {
        this.random = new Random(1);

        if (!await this.db.Set<TEntity>().AnyAsync(cancellationToken))
        {
            await this.db.BulkInsertAsync(entities, cancellationToken: cancellationToken);
        }
    }

    private async Task SeedAttendanceCodes(CancellationToken cancellationToken = default) =>
        await this.BulkSeed(Enumerable.Range(0, 1_000_000)
            .OrderBy(_ => this.random.NextDouble())
            .Select((v, index) => new AttendanceCode
            {
                Id = index + 1, Code = v.ToString("D6", CultureInfo.InvariantCulture)
            }), cancellationToken);
}
