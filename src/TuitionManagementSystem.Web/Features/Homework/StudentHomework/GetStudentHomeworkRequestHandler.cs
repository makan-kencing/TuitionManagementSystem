namespace TuitionManagementSystem.Web.Features.Homework.StudentHomework;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetStudentHomeworkRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetStudentHomeworkRequest, Result<GetStudentHomeworkResponse>>
{
    public async Task<Result<GetStudentHomeworkResponse>> Handle(GetStudentHomeworkRequest request,
        CancellationToken cancellationToken)
    {
        var studentHomeworks = await db.Students
            .Where(s => s.Id == request.UserId)
            .Select(s => new GetStudentHomeworkResponse
            {
                Name = s.Account.DisplayName,
                CourseInfos = s.Enrollments
                    .Select(e => new StudentCourseInfo
                    {
                        Id = e.Course.Id,
                        EnrollDate = e.EnrolledAt,
                        Name = e.Course.Name,
                        SubjectName = e.Course.Subject.Name,
                        Teacher = e.Course.TeachersInCharge
                            .Select(ct => new CourseTeacherInfo
                            {
                                Id = ct.Teacher.Id,
                                Name = ct.Teacher.Account.DisplayName
                            })
                            .First()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (studentHomeworks is null)
        {
            return Result.NotFound();
        }

        return Result.Success(studentHomeworks);
    }
}

