using MediatR;
using TmsApi.Application.Dtos;
using TmsApi.Application.Services;

namespace TmsApi.Application.Courses.Queries;

public class GetCoursesHandler(
    ICachedCourseService cachedCourseService)
    : IRequestHandler<GetCoursesQuery, List<CourseDto>>
{
    public Task<List<CourseDto>> Handle(
        GetCoursesQuery query,
        CancellationToken ct)
    {
        return cachedCourseService.GetAllCoursesAsync(ct);
    }
}