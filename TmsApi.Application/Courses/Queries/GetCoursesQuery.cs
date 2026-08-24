using MediatR;
using TmsApi.Application.Dtos;

namespace TmsApi.Application.Courses.Queries;

public record GetCoursesQuery : IRequest<List<CourseDto>>;