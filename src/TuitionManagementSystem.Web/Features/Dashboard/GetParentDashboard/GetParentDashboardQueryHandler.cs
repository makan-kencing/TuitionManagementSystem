namespace TuitionManagementSystem.Web.Features.Dashboard.GetParentDashboard;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class.Announcement;
using Schedule;

public class GetParentDashboardQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetParentDashboardQuery, Result<GetParentDashboardResponse>>
{
    public async Task<Result<GetParentDashboardResponse>> Handle(GetParentDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var family = await db.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => u.Family)
            .FirstOrDefaultAsync(cancellationToken);

        if (family is null)
        {
            return Result.NotFound();
        }

        var timezone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
        var hour0 = DateTimeUtc.ToUtcAssumingLocal(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone).Date);
        var hour24 = hour0.AddDays(1);

        var stats = new GetParentDashboardResponse
        {
            TotalChildren = await db.Students
                .Where(s => s.Family.Id == family.FamilyId)
                .CountAsync(cancellationToken),
            PendingClasses = await db.Students
                .Where(s => s.Family.Id == family.FamilyId)
                .SelectMany(s => s.Enrollments)
                .SelectMany(e => e.Course.Sessions)
                .Where(s => hour0 <= s.StartAt && s.StartAt <= hour24)
                .CountAsync(cancellationToken),
            PendingHomework = await db.Database
                .SqlQuery<int>(
                    $"""
                     SELECT COUNT(a."Id") - COUNT(s."Id") "Value"
                     FROM "Users" u
                     JOIN "FamilyMembers" fm on fm."UserId" = u."Id"
                     JOIN "Enrollments" e on e."StudentId" = u."Id"
                         JOIN "Courses" c  on c."Id" = e."CourseId"
                             JOIN "Announcements" a on a."CourseId" = c."Id"
                                 LEFT JOIN "Submissions" s on s."StudentId" = u."Id" and s."AssignmentId" = a."Id"
                     WHERE u."Discriminator" = 'Student' AND fm."FamilyId" = {family.FamilyId} AND a."Discriminator" = 'Assignment'
                     """)
                .FirstAsync(cancellationToken),
            PendingInvoices = await db.Database
                .SqlQuery<PendingInvoiceDetails>(
                    $"""
                     SELECT COALESCE(sum(i."Amount"), 0.0) AS "Total", count(*)::int AS "Count"
                     FROM "Users" AS u
                              LEFT JOIN "FamilyMembers" AS f ON u."Id" = f."UserId"
                              INNER JOIN "Invoices" AS i ON u."Id" = i."StudentId"
                     WHERE u."Discriminator" = 'Student' AND f."Id" = {family.FamilyId}
                     """)
                .FirstAsync(cancellationToken),
            ChildrenAttendanceRate = await db.Database
                .SqlQuery<RateRow>(
                    $"""
                     SELECT coalesce(acc."DisplayName", acc."Username") "Name", COUNT(a."Id") * 100 / COUNT(s."Id") "Rate"
                     FROM "Users" u
                     JOIN "Accounts" acc on acc."Id" = u."AccountId"
                     JOIN "FamilyMembers" fm on fm."UserId" = u."Id"
                     JOIN "Enrollments" e on e."StudentId" = u."Id"
                         JOIN "Courses" c  on c."Id" = e."CourseId"
                             JOIN "Sessions" s on s."CourseId" = c."Id"
                                 LEFT JOIN "Attendances" a on a."StudentId" = u."Id" and a."SessionId" = s."Id"
                     WHERE u."Discriminator" = 'Student' AND fm."FamilyId" = {family.FamilyId}
                     GROUP BY acc."DisplayName", acc."Username"
                     """)
                .ToDictionaryAsync(g => g.Name, g => g.Rate, cancellationToken),
            ChildrenSubmissionRate = await db.Database
                .SqlQuery<RateRow>(
                    $"""
                     SELECT coalesce(acc."DisplayName", acc."Username") "Name", COUNT(s."Id") * 100 / COUNT(a."Id") "Rate"
                     FROM "Users" u
                     JOIN "Accounts" acc on acc."Id" = u."AccountId"
                     JOIN "FamilyMembers" fm on fm."UserId" = u."Id"
                     JOIN "Enrollments" e on e."StudentId" = u."Id"
                         JOIN "Courses" c  on c."Id" = e."CourseId"
                             JOIN "Announcements" a on a."CourseId" = c."Id"
                                 LEFT JOIN "Submissions" s on s."StudentId" = u."Id" and s."AssignmentId" = a."Id"
                     WHERE u."Discriminator" = 'Student' AND fm."FamilyId" = {family.FamilyId} AND a."Discriminator" = 'Assignment'
                     GROUP BY acc."DisplayName", acc."Username"
                     """)
                .ToDictionaryAsync(g => g.Name, g => g.Rate, cancellationToken)
        };

        return Result.Success(stats);
    }

    private record RateRow(string Name, int Rate);
}
