namespace TuitionManagementSystem.Web.Infrastructure.Persistence;

using System.Globalization;
using EFCore.BulkExtensions;
using Htmx.Net.Toast.Helpers;
using Ical.Net;
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
    private readonly List<Schedule> schedules;
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
            new Classroom { Location = "Room E",  MaxCapacity = 15 },
            new Classroom { Location = "Room F",  MaxCapacity = 15 },
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
        Price = 100.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(14),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Monday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(2).AddHours(14),
                EndAt = DateTime.UtcNow.Date.AddDays(2).AddHours(16),
                Classroom = this.classrooms[0]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(9).AddHours(14),
                EndAt = DateTime.UtcNow.Date.AddDays(9).AddHours(16),
                Classroom = this.classrooms[0]
            }
        ]
    },

    new Course
    {
        Name = "General Science",
        Subject = this.subjects[1],
        PreferredClassroom = this.classrooms[1],
        Price = 120.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(12),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Wednesday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(4).AddHours(10),
                EndAt = DateTime.UtcNow.Date.AddDays(4).AddHours(12),
                Classroom = this.classrooms[1]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(11).AddHours(10),
                EndAt = DateTime.UtcNow.Date.AddDays(11).AddHours(12),
                Classroom = this.classrooms[1]
            }
        ]
    },

    new Course
    {
        Name = "English Grammar",
        Subject = this.subjects[2],
        PreferredClassroom = this.classrooms[2],
        Price = 110.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(17).AddMinutes(30),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Tuesday, DayOfWeek.Thursday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(3).AddHours(16),
                EndAt = DateTime.UtcNow.Date.AddDays(3).AddHours(17).AddMinutes(30),
                Classroom = this.classrooms[2]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(5).AddHours(16),
                EndAt = DateTime.UtcNow.Date.AddDays(5).AddHours(17).AddMinutes(30),
                Classroom = this.classrooms[2]
            }
        ]
    },

    new Course
    {
        Name = "Sejarah",
        Subject = this.subjects[3],
        PreferredClassroom = this.classrooms[6],
        Price = 95.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(11),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Friday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(6).AddHours(9),
                EndAt = DateTime.UtcNow.Date.AddDays(6).AddHours(11),
                Classroom = this.classrooms[6]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(13).AddHours(9),
                EndAt = DateTime.UtcNow.Date.AddDays(13).AddHours(11),
                Classroom = this.classrooms[6]
            }
        ]
    },

    new Course
    {
        Name = "Nuclear Physics",
        Subject = this.subjects[4],
        PreferredClassroom = this.classrooms[4],
        Price = 130.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(18),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(21),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Monthly,
                    ByDay = [DayOfWeek.Monday],
                    BySetPosition = [1]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(15).AddHours(18),
                EndAt = DateTime.UtcNow.Date.AddDays(15).AddHours(21),
                Classroom = this.classrooms[4]
            }
        ]
    },

    new Course
    {
        Name = "Renaissance Art",
        Subject = this.subjects[5],
        PreferredClassroom = this.classrooms[7],
        Price = 140.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(13),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Saturday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(7).AddHours(13),
                EndAt = DateTime.UtcNow.Date.AddDays(7).AddHours(16),
                Classroom = this.classrooms[7]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(14).AddHours(13),
                EndAt = DateTime.UtcNow.Date.AddDays(14).AddHours(16),
                Classroom = this.classrooms[7]
            }
        ]
    },

    new Course
    {
        Name = "Python for Beginners",
        Subject = this.subjects[6],
        PreferredClassroom = this.classrooms[5],
        Price = 150.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(19),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(21),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Tuesday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(3).AddHours(19),
                EndAt = DateTime.UtcNow.Date.AddDays(3).AddHours(21),
                Classroom = this.classrooms[5]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(10).AddHours(19),
                EndAt = DateTime.UtcNow.Date.AddDays(10).AddHours(21),
                Classroom = this.classrooms[5]
            }
        ]
    },

    new Course
    {
        Name = "Physical Geography",
        Subject = this.subjects[7],
        PreferredClassroom = this.classrooms[8],
        Price = 105.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(15),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(17),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Thursday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(5).AddHours(15),
                EndAt = DateTime.UtcNow.Date.AddDays(5).AddHours(17),
                Classroom = this.classrooms[8]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(12).AddHours(15),
                EndAt = DateTime.UtcNow.Date.AddDays(12).AddHours(17),
                Classroom = this.classrooms[8]
            }
        ]
    },


    new Course
    {
        Name = "Statistics",
        Subject = this.subjects[8],
        PreferredClassroom = this.classrooms[7],
        Price = 115.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(12),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Saturday, DayOfWeek.Sunday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(7).AddHours(9),
                EndAt = DateTime.UtcNow.Date.AddDays(7).AddHours(12),
                Classroom = this.classrooms[7]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(8).AddHours(9),
                EndAt = DateTime.UtcNow.Date.AddDays(8).AddHours(12),
                Classroom = this.classrooms[7]
            }
        ]
    },

    new Course
    {
        Name = "Organic Chemistry",
        Subject = this.subjects[9],
        PreferredClassroom = this.classrooms[4],
        Price = 135.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(14),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Monday, DayOfWeek.Wednesday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(2).AddHours(14),
                EndAt = DateTime.UtcNow.Date.AddDays(2).AddHours(16),
                Classroom = this.classrooms[4]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(4).AddHours(14),
                EndAt = DateTime.UtcNow.Date.AddDays(4).AddHours(16),
                Classroom = this.classrooms[4]
            }
        ]
    },

    new Course
    {
        Name = "Calculus I",
        Subject = this.subjects[0],
        PreferredClassroom = this.classrooms[3],
        Price = 125.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(18),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(20),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Friday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(6).AddHours(18),
                EndAt = DateTime.UtcNow.Date.AddDays(6).AddHours(20),
                Classroom = this.classrooms[3]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(13).AddHours(18),
                EndAt = DateTime.UtcNow.Date.AddDays(13).AddHours(20),
                Classroom = this.classrooms[3]
            }
        ]
    },

    new Course
    {
        Name = "Creative Writing",
        Subject = this.subjects[2],
        PreferredClassroom = this.classrooms[9],
        Price = 110.00m,
        Schedule = new Schedule
        {
            Start = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            End = DateTime.UtcNow.Date.AddDays(1).AddHours(18),
            RecurrencePatterns =
            [
                new ScheduleRecurrencePattern
                {
                    FrequencyType = FrequencyType.Weekly,
                    ByDay = [DayOfWeek.Wednesday]
                }
            ]
        },
        Sessions =
        [
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(4).AddHours(16),
                EndAt = DateTime.UtcNow.Date.AddDays(4).AddHours(18),
                Classroom = this.classrooms[9]
            },
            new Session
            {
                StartAt = DateTime.UtcNow.Date.AddDays(11).AddHours(16),
                EndAt = DateTime.UtcNow.Date.AddDays(11).AddHours(18),
                Classroom = this.classrooms[9]
            }
        ]
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

        this.schedules = this.courses
            .Select(c =>
            {
                var start = DateTime.UtcNow.Date.AddDays(1);
                return new Schedule
                {
                    Summary = null,
                    Description = null,
                    Start = start,
                    End = start.AddHours(2),
                    Course = c
                };

            })
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
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await this.SeedAttendanceCodes(cancellationToken);
        await this.Seed(this.subjects, cancellationToken);
        await this.Seed(this.classrooms, cancellationToken);
        await this.Seed(this.courses, cancellationToken);
        await this.Seed(this.schedules, cancellationToken);
        await this.Seed(this.Users, cancellationToken);
        await this.Seed(this.families, cancellationToken);
        await this.Seed(this.enrollments, cancellationToken);
        await this.Seed(this.sessions, cancellationToken);
        await this.Seed(this.attendances, cancellationToken);
        await this.Seed(this.courseTeachers, cancellationToken);
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
