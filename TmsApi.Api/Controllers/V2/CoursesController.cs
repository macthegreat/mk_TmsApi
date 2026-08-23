using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Courses.Commands;
using TmsApi.Application.Courses.Queries;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/courses")]
[ApiVersion("2.0")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCourses(
        CancellationToken ct)
    {
        var courses = await mediator.Send(
            new GetCoursesQuery(),
            ct);

        return Ok(courses);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCourse(
        int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken ct)
    {
        var command = new UpdateCourseCommand(
            id,
            request.Title);

        var updated = await mediator.Send(command, ct);

        return updated
            ? NoContent()
            : NotFound();
    }
}

public record UpdateCourseRequest(string Title);