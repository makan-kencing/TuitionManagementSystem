namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Globalization;
using EFCore.BulkExtensions;
using Htmx.Net.Toast.Helpers;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Models.User;
using Services.Auth.Constants;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using NuGet.Protocol;

public class SeedData
{
    private Random random = new(1);

    private readonly List<Subject> subjects;
    private readonly List<Classroom> classrooms;
    private readonly List<Teacher> teachers;
    private readonly List<Student> students;
    private readonly List<Parent> parents;
    private readonly List<Family> families;
    private readonly List<Course> courses;
    private readonly List<Enrollment> enrollments;
    private readonly List<Session> sessions;
    private readonly List<Attendance> attendances;
    private readonly List<CourseTeacher> courseTeachers;

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

        {
            var studentsCursor = this.students.GetEnumerator().ToIEnumerable();
            this.families = this.parents
                .Select((p, i) => new Family
                {
                    Name = $"Family {i + 1}",
                    Members = studentsCursor.Take(this.random.Next(0, 3))
                        .Select(s => new FamilyMember { User = s })
                        .Append(new FamilyMember { User = p }).ToList()
                })
                .ToList();
        }

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

        this.sessions = this.courses
            .Select(c => Enumerable.Range(1, 5)
                .Select(i =>
                {
                    var start = DateTime.UtcNow.Date
                        .AddDays(i * 2);

                    return new Session
                    {
                        Course = c,
                        Classroom = c.PreferredClassroom,
                        StartAt = start,
                        EndAt = start.AddHours(2)
                    };
                }))
            .SelectMany(i => i)
            .ToList();

        this.attendances = this.enrollments
            .Select(e => e.Course.Sessions
                .Select(s => new Attendance
                {
                    Student = e.Student,
                    Session = s,
                    TakenOn = s.StartAt.AddMinutes(this.random.Next(0, 10))
                }))
            .SelectMany(i => i)
            .ToList();

        {
            var coursesCursor = this.courses.GetEnumerator().ToIEnumerable();
            this.courseTeachers = this.teachers
                .Select(t => coursesCursor.Take(this.random.Next(1, 3))
                    .Select(c => new CourseTeacher { Course = c, Teacher = t }))
                .SelectMany(i => i)
                .ToList();
            Console.WriteLine(this.courseTeachers.ToJson(Formatting.Indented));
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await this.SeedAttendanceCodes(cancellationToken);
        await this.Seed(this.subjects, cancellationToken);
        await this.Seed(this.classrooms, cancellationToken);
        await this.Seed(this.courses, cancellationToken);
        await this.Seed(this.Users, cancellationToken);
        await this.Seed(this.families, cancellationToken);
        await this.Seed(this.enrollments, cancellationToken);
        await this.Seed(this.sessions, cancellationToken);
        await this.BulkSeed(this.attendances, cancellationToken);
        await this.BulkSeed(this.courseTeachers, cancellationToken);
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

internal static class IEnumeratorExtensions
{
    public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator) {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}
