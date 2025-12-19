namespace TuitionManagementSystem.Web.Features.Homework.MakeAnnouncement;

using Ardalis.Result;
using MediatR;

public record MakeAnnouncementInfoRequest(
    int CourseId,
    int UserId,
    ICollection<int> FileIds,
    string Title,
    string? Description,
    DateTime? DueAt)
    : IRequest<Result<MakeAnnouncementInfoResponse>>;
