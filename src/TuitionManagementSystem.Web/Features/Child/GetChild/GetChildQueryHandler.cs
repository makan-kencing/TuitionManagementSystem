namespace TuitionManagementSystem.Web.Features.Child.GetChild;

using Ardalis.Result;
using MediatR;

public class GetChildQueryHandler : IRequestHandler<GetChildQuery, Result<GetChildResponse>>
{
    public async Task<Result<GetChildResponse>> Handle(GetChildQuery request, CancellationToken cancellationToken)
    {

        return Result.Success();
    }
}
