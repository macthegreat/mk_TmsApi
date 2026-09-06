using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Courses.Commands;
using TmsApi.Application.Courses.Queries;
using TmsApi.Api.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/courses")]
[ApiVersion("2.0")]
public class CoursesController(
    IMediator mediator,
    TmsDbContext context,
    IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCourses(CancellationToken ct)
    {
        var courses = await mediator.Send(
            new GetCoursesQuery(),
            ct);

        return Ok(courses);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> UpdateCourse(
        int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken ct)
    {
        var course = await context.Courses.FindAsync(
            new object[] { id }, ct);

        if (course is null)
            return NotFound();

        var authResult = await authorizationService.AuthorizeAsync(
            User,
            course,
            "CanEditCourse");

        if (!authResult.Succeeded)
            return Forbid();

        var command = new UpdateCourseCommand(
            id,
            request.Title);

        var updated = await mediator.Send(command, ct);

        return updated
            ? NoContent()
            : NotFound();
    }

    [HttpGet("search")]
    [EnableRateLimiting("search")]
    public async Task<IActionResult> SearchCourses(
        [FromQuery] string? term,
        CancellationToken ct)
    {
        var results = await mediator.Send(
            new SearchCoursesQuery(term),
            ct);

        return Ok(results);
    }
}

public record UpdateCourseRequest(string Title);






















// using Asp.Versioning;
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using TmsApi.Application.Courses.Commands;
// using TmsApi.Application.Courses.Queries;
// using TmsApi.Api.RateLimiting;
// using Microsoft.AspNetCore.RateLimiting;
// using System.Threading.RateLimiting;
// using Microsoft.AspNetCore.Authorization;




// namespace TmsApi.Api.Controllers.V2;

// [ApiController]
// [Route("api/v{version:apiVersion}/courses")]
// [ApiVersion("2.0")]
// public class CoursesController(IMediator mediator) : ControllerBase
// {
//     [HttpGet]
//     public async Task<IActionResult> GetCourses(
//         CancellationToken ct)
//     {
//         var courses = await mediator.Send(
//             new GetCoursesQuery(),
//             ct);

//         return Ok(courses);
//     }

//     [HttpPut("{id:int}")]
//     public async Task<IActionResult> UpdateCourse(
//         int id,
//         [FromBody] UpdateCourseRequest request,
//         CancellationToken ct)
//     {
//         var command = new UpdateCourseCommand(
//             id,
//             request.Title);

//         var updated = await mediator.Send(command, ct);

//         return updated
//             ? NoContent()
//             : NotFound();
//     }
//     [HttpGet("search")]
//     [EnableRateLimiting("search")]
//     public async Task<IActionResult> SearchCourses(
//     [FromQuery] string? term,
//     CancellationToken ct)
//     {
//         var results = await mediator.Send(
//             new SearchCoursesQuery(term),
//             ct);

//         return Ok(results);
//     }
// }

// public record UpdateCourseRequest(string Title);