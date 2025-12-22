namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Globalization;
using EFCore.BulkExtensions;
using Ical.Net;
using Ical.Net.DataTypes;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Models.User;
using Services.Auth.Constants;
using Microsoft.AspNetCore.Identity;
using Models.Class.Announcement;
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
    private readonly List<Announcement> announcements;
    private readonly List<Submission> submissions;

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
            new Subject { Name = "Advanced English" },
            new Subject { Name = "History" },
            new Subject { Name = "Physics" },
            new Subject { Name = "Art" },
            new Subject { Name = "Computer Science" },
            new Subject { Name = "Geography" },
            new Subject { Name = "Add Math" },
            new Subject { Name = "Chemistry" }
        ];

        this.classrooms =
        [
            new Classroom { Location = "Room A", MaxCapacity = 30 },
            new Classroom { Location = "Room B", MaxCapacity = 25 },
            new Classroom { Location = "Room C", MaxCapacity = 20 },
            new Classroom { Location = "Room D", MaxCapacity = 35 },
            new Classroom { Location = "Room E", MaxCapacity = 15 },
            new Classroom { Location = "Room F", MaxCapacity = 15 },
            new Classroom { Location = "Room G", MaxCapacity = 50 },
            new Classroom { Location = "Room H", MaxCapacity = 12 },
            new Classroom { Location = "Room I", MaxCapacity = 28 },
            new Classroom { Location = "Room J", MaxCapacity = 22 }
        ];

        this.courses =
[
    new Course
    {
        Name = "Basic Algebra",
        Subject = this.subjects[0],
        PreferredClassroom = this.classrooms[0],
        Price = 85.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-1).AddHours(14),
            End = DateTime.UtcNow.Date.AddDays(-1).AddHours(16),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Monday]
                }
            ]
        }
    },

    new Course
    {
        Name = "General Science",
        Subject = this.subjects[1],
        PreferredClassroom = this.classrooms[1],
        Price = 75.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-5).AddHours(10),
            End = DateTime.UtcNow.Date.AddDays(-5).AddHours(12),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Wednesday]
                }
            ]
        }
    },

    new Course
    {
        Name = "English Grammar",
        Subject = this.subjects[2],
        PreferredClassroom = this.classrooms[2],
        Price = 89.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-13).AddHours(16),
            End = DateTime.UtcNow.Date.AddDays(-13).AddHours(17).AddMinutes(30),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Tuesday, DayOfWeek.Thursday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Sejarah",
        Subject = this.subjects[3],
        PreferredClassroom = this.classrooms[6],
        Price = 65.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-9).AddHours(9),
            End = DateTime.UtcNow.Date.AddDays(-9).AddHours(11),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Friday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Nuclear Physics",
        Subject = this.subjects[4],
        PreferredClassroom = this.classrooms[4],
        Price = 99.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-6).AddHours(18),
            End = DateTime.UtcNow.Date.AddDays(-6).AddHours(21),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Monthly, ByDay = [DayOfWeek.Monday], BySetPosition = [1]
                }
            ]
        }
    },

    new Course
    {
        Name = "Renaissance Art",
        Subject = this.subjects[5],
        PreferredClassroom = this.classrooms[7],
        Price = 82.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-7).AddHours(13),
            End = DateTime.UtcNow.Date.AddDays(-7).AddHours(16),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Saturday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Python for Beginners",
        Subject = this.subjects[6],
        PreferredClassroom = this.classrooms[5],
        Price = 95.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-8).AddHours(19),
            End = DateTime.UtcNow.Date.AddDays(-8).AddHours(21),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Tuesday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Physical Geography",
        Subject = this.subjects[7],
        PreferredClassroom = this.classrooms[8],
        Price = 70.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-15).AddHours(15),
            End = DateTime.UtcNow.Date.AddDays(-15).AddHours(17),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Thursday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Statistics",
        Subject = this.subjects[8],
        PreferredClassroom = this.classrooms[7],
        Price = 98.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-17).AddHours(9),
            End = DateTime.UtcNow.Date.AddDays(-17).AddHours(12),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Saturday, DayOfWeek.Sunday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Organic Chemistry",
        Subject = this.subjects[9],
        PreferredClassroom = this.classrooms[4],
        Price = 94.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-10).AddHours(14),
            End = DateTime.UtcNow.Date.AddDays(-10).AddHours(16),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Monday, DayOfWeek.Wednesday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Calculus I",
        Subject = this.subjects[0],
        PreferredClassroom = this.classrooms[3],
        Price = 88.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-4).AddHours(18),
            End = DateTime.UtcNow.Date.AddDays(-4).AddHours(20),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Friday]
                }
            ]
        }
    },

    new Course
    {
        Name = "Creative Writing",
        Subject = this.subjects[2],
        PreferredClassroom = this.classrooms[9],
        Price = 79.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(-12).AddHours(16),
            End = DateTime.UtcNow.Date.AddDays(-12).AddHours(18),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly, ByDay = [DayOfWeek.Wednesday]
                }
            ]
        }
    }
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
                        DisplayName = $"Teacher {i}",
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
                .Select(c =>
                {
                    var enrollment = new Enrollment
                    {
                        Student = s, Course = c, EnrolledAt = DateTime.UtcNow.AddDays(-this.random.Next(1, 30))
                    };
                    s.Enrollments.Add(enrollment);
                    c.Enrollments.Add(enrollment);
                    return enrollment;
                })
            )
            .SelectMany(i => i)
            .ToList();

        this.sessions = this.courses
            .Where(c => c.Schedule != null)
            .Select(c => c.Schedule!.ToICalendarEvent()
                .GetOccurrences()
                .TakeWhileBefore(CalDateTime.UtcNow)
                .Select(o =>
                {
                    var session = new Session
                    {
                        Course = c,
                        Classroom = c.PreferredClassroom,
                        StartAt = o.Period.StartTime.AsUtc,
                        EndAt = o.Period.EffectiveEndTime!.AsUtc
                    };
                    c.Sessions.Add(session);
                    return session;
                }))
            .SelectMany(i => i)
            .ToList();

        this.attendances = this.enrollments
            .Select(e => e.Course.Sessions
                .Select(s => this.random.Next(1, 11) switch
                {
                    1 => null,
                    _ => new Attendance
                    {
                        Student = e.Student, Session = s, TakenOn = s.StartAt.AddMinutes(this.random.Next(0, 10))
                    }
                }))
            .SelectMany(i => i)
            .OfType<Attendance>()
            .ToList();

        {
            var coursesCursor = this.courses.GetEnumerator().ToIEnumerable();
            this.courseTeachers = this.teachers
                .Select(t => coursesCursor.Take(this.random.Next(1, 3))
                    .Select(c =>
                    {
                        var ct = new CourseTeacher { Course = c, Teacher = t };
                        c.TeachersInCharge.Add(ct);
                        t.Courses.Add(ct);
                        return ct;
                    }))
                .SelectMany(i => i)
                .ToList();
        }

        this.announcements = this.courses
            .Select(c => Enumerable.Range(1, this.random.Next(5))
                .Select(i => this.random.Next(1, 3) switch
                {
                    1 => new Announcement
                    {
                        Title = $"Announcement {i + 1} - {c.Name}",
                        Description = "Below are the following contents.",
                        Course = c,
                        CreatedBy = c.TeachersInCharge.ElementAt(this.random.Next(c.TeachersInCharge.Count)).Teacher
                    },
                    _ => new Assignment
                    {
                        Title = $"Assignment {i + 1} - {c.Name}",
                        Description = "Submit the assignment before the due date.",
                        Course = c,
                        CreatedBy =
                            c.TeachersInCharge.ElementAt(this.random.Next(c.TeachersInCharge.Count)).Teacher,
                        DueAt = DateTime.UtcNow.AddDays(2)
                    }
                }))
            .SelectMany(i => i)
            .ToList();

        this.submissions = this.announcements
            .OfType<Assignment>()
            .Select(a => a.Course.Enrollments
                .Select(e => this.random.Next(1, 10) switch
                {
                    1 => null,
                    _ => new Submission { Student = e.Student, Assignment = a, Content = "Homework completed" }
                }))
            .SelectMany(i => i)
            .OfType<Submission>()
            .ToList();
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
        await this.Seed(this.attendances, cancellationToken);
        await this.Seed(this.courseTeachers, cancellationToken);
        await this.Seed(this.announcements, cancellationToken);
        await this.Seed(this.submissions, cancellationToken);
    }

    private async Task Seed<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        this.random = new Random(1);

        if (!await this.db.Set<TEntity>().AnyAsync(cancellationToken))
        {
            await this.db.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            await this.db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task BulkSeed<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class
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
    public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}
