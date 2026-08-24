using MediatR;
using TmsApi.Application.Dtos;
using TmsApi.Application.Services;

namespace TmsApi.Application.Courses.Queries;

public class SearchCoursesHandler(
    ICourseService courseService)
    : IRequestHandler<SearchCoursesQuery, IReadOnlyList<CourseResponseDto>>
{
    public async Task<IReadOnlyList<CourseResponseDto>> Handle(
        SearchCoursesQuery query,
        CancellationToken ct)
    {
        var request = new PagedRequest
        {
            Page = 1,
            PageSize = 50,
            Search = query.Term
        };

        var result = await courseService.GetCoursesAsync(
            request,
            ct);

        return result.Items;
    }
}