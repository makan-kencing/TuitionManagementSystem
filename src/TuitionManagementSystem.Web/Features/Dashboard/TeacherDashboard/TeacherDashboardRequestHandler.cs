namespace TuitionManagementSystem.Web.Features.Dashboard.TeacherDashboard
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using TuitionManagementSystem.Web.Infrastructure.Persistence;
    using TuitionManagementSystem.Web.Models.Class;

    public class TeacherDashboardRequestHandler(ApplicationDbContext db)
        : IRequestHandler<TeacherDashboardRequest, TeacherDashboardResponse>
    {
        public async Task<TeacherDashboardResponse> Handle(TeacherDashboardRequest request,
            CancellationToken cancellationToken)
        {
            var todayUtc = DateTime.UtcNow.Date;
            var tomorrowUtc = todayUtc.AddDays(1);

            var teacherCourses = await db.CourseTeachers
                .Where(ct => ct.TeacherId == request.TeacherId)
                .Select(ct => new { ct.CourseId, ct.Course })
                .ToListAsync(cancellationToken);

            if (!teacherCourses.Any())
            {
                return new TeacherDashboardResponse
                {
                    TotalSessionsToday = 0,
                    ClassesToTeachToday = 0,
                    TotalStudents = 0,
                    HomeworkPending = 0,
                    AttendancePerSession = new Dictionary<string, int> { { "No Sessions", 0 } },
                    SubmissionPerCourse = new Dictionary<string, int> { { "No Courses", 0 } },
                    AverageAttendancePerCourse = new Dictionary<string, int>(),
                    CourseCapacity = new Dictionary<string, int>()
                };
            }

            var courseIds = teacherCourses.Select(tc => tc.CourseId).ToList();

            var sessionsToday = await db.Sessions
                .Where(s => courseIds.Contains(s.CourseId) &&
                            s.StartAt >= todayUtc &&
                            s.StartAt < tomorrowUtc)
                .ToListAsync(cancellationToken);

            var totalStudentsInCourses = await db.Enrollments
                .Where(e => courseIds.Contains(e.CourseId))
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            var courseAttendanceData = await db.Sessions
                .Include(s => s.Course)
                .Include(s => s.Attendances)
                .Where(s => courseIds.Contains(s.CourseId))
                .GroupBy(s => new { s.CourseId, s.Course.Name })
                .Select(g => new
                {
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.Name,
                    TotalSessions = g.Count(),
                    TotalAttendance = g.Sum(s => s.Attendances.Count),
                    MaxAttendance = g.Max(s => s.Attendances.Count)
                })
                .ToListAsync(cancellationToken);

            var courseEnrollments = await db.Enrollments
                .Where(e => courseIds.Contains(e.CourseId))
                .GroupBy(e => new { e.CourseId, e.Course.Name })
                .Select(g => new
                {
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.Name,
                    EnrollmentCount = g.Count()
                })
                .ToDictionaryAsync(x => x.CourseId, x => x, cancellationToken);

            var response = new TeacherDashboardResponse
            {
                TotalSessionsToday = sessionsToday.Count,
                ClassesToTeachToday = sessionsToday.Count,
                TotalStudents = totalStudentsInCourses,
                HomeworkPending = await db.Assignments
                    .Where(a => courseIds.Contains(a.CourseId) &&
                                !a.Submissions.Any())
                    .CountAsync(cancellationToken),
                AttendancePerSession = new Dictionary<string, int>(),
                SubmissionPerCourse = new Dictionary<string, int>(),
                AverageAttendancePerCourse = new Dictionary<string, int>(),
                CourseCapacity = new Dictionary<string, int>()
            };

            foreach (var courseData in courseAttendanceData)
            {
                var courseName = courseData.CourseName;

                if (courseData.TotalSessions > 0)
                {
                    var averageAttendance = (int)Math.Round((double)courseData.TotalAttendance / courseData.TotalSessions);
                    response.AverageAttendancePerCourse.Add(courseName, averageAttendance);
                }
                else
                {
                    response.AverageAttendancePerCourse.Add(courseName, 0);
                }

                int capacity = 0;
                if (courseEnrollments.TryGetValue(courseData.CourseId, out var enrollmentData))
                {
                    capacity = enrollmentData.EnrollmentCount;
                    response.CourseCapacity.Add(courseName, capacity);
                }
                else
                {
                    capacity = courseData.MaxAttendance;
                    response.CourseCapacity.Add(courseName, capacity);
                }

                if (capacity > 0)
                {
                    var attendancePercentage = (int)Math.Round((double)response.AverageAttendancePerCourse[courseName] / capacity * 100);
                }
            }

            var allSessions = await db.Sessions
                .Include(s => s.Course)
                .Where(s => courseIds.Contains(s.CourseId))
                .Take(10)
                .ToListAsync(cancellationToken);

            foreach (var session in allSessions)
            {
                var attendanceCount = await db.Attendances
                    .CountAsync(a => a.SessionId == session.Id, cancellationToken);
                response.AttendancePerSession.Add(
                    $"{session.Course.Name} ({session.StartAt:yyyy-MM-dd HH:mm})",
                    attendanceCount);
            }

            foreach (var teacherCourse in teacherCourses)
            {
                var courseId = teacherCourse.CourseId;
                var course = teacherCourse.Course;

                if (course != null)
                {
                    var totalAssignments = await db.Assignments
                        .CountAsync(a => a.CourseId == courseId, cancellationToken);

                    if (totalAssignments > 0)
                    {
                        var submittedCount = await db.Submissions
                            .CountAsync(s => s.Assignment.CourseId == courseId, cancellationToken);

                        var percentage = (int)Math.Round((double)submittedCount / totalAssignments * 100);
                        response.SubmissionPerCourse.Add(course.Name, percentage);
                    }
                    else
                    {
                        response.SubmissionPerCourse.Add(course.Name, 0);
                    }
                }
            }

            if (!response.AttendancePerSession.Any())
                response.AttendancePerSession.Add("No Sessions", 0);

            if (!response.SubmissionPerCourse.Any())
                response.SubmissionPerCourse.Add("No Courses", 0);

            if (!response.AverageAttendancePerCourse.Any())
            {
                response.AverageAttendancePerCourse.Add("No Courses", 0);
                response.CourseCapacity.Add("No Courses", 0);
            }

            return response;
        }
    }
}
