using Microsoft.AspNetCore.Mvc;
using TmsApi.Domain.Entities;
using TmsApi.Application.Services;
using TmsApi.Application.Dtos;



namespace TmsApi.Api.Controllers;

[ApiController]
//[Route("api/enrollments")]
[Route("api/courses/{courseId:int}/enrollments")]
[Produces("application/json")] [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class EnrollmentsController(ICourseService courseService , IEnrollmentService enrollmentService) : ControllerBase
{
    

//     [HttpGet]
//     public async Task<IActionResult> GetAll()
//     {
//         var enrollments = await enrollmentService.GetAllAsync();
//         return Ok(enrollments);
//     }

//     [HttpGet("{id}")]

//     public async Task<IActionResult> GetById(string id)
//     {
//         var record = await enrollmentService.GetByIdAsync(id);
//         return record is not null ? Ok(record) : NotFound();
//     }

//    [HttpPost("~/api/enrollments")]
//     public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
//     {
//         var record = await enrollmentService.EnrollAsync(request.StudentId, request.CourseCode);
//         return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
//     }

//    [HttpDelete("~/api/enrollments/{id}")]

//     public async Task<IActionResult> Delete(string id)
//     {
//         var deleted = await enrollmentService.DeleteAsync(id);
//         return deleted ? NoContent() : NotFound();
//     }

/// stsrt from here
 [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
 [ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get one enrolment for a course")]
    public async Task<IActionResult> GetEnrollment(
        int courseId,
        int id,
        CancellationToken ct)
    {
        var enrollment = await enrollmentService.GetByIdAsync(
            courseId,
            id,
            ct);

        return enrollment is not null
            ? Ok(enrollment)
            : NotFound();
    }

[HttpPost]
[ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes. Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[EndpointSummary("Enrol a student in a course")] 
[EndpointDescription("Returns 404 if the course does not exist, 409 if the course has reached MaxCapacity.")]
    public async Task<IActionResult> EnrollStudent(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct)
    {
        // 404 FIRST
        var course = await courseService.GetByIdAsync(
            courseId,
            ct);

        if (course is null)
        {
            return NotFound();
        }

        // 409 SECOND
        var enrollmentCount = course.Enrollments.Count;

        if (enrollmentCount >= course.MaxCapacity)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course is full",
                Detail =
                    $"Course '{course.Title}' has reached its maximum capacity of {course.MaxCapacity}.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var enrollment = await enrollmentService.CreateAsync(
            courseId,
            request,
            ct);

            return CreatedAtAction(
            nameof(GetEnrollment),
            new
            {
                courseId,
                id = enrollment.Id
            },
            enrollment);
    }
[HttpGet(Name = "ListCourseEnrollments")]

[ProducesResponseType(typeof(IReadOnlyList<EnrollmentResponseDto>),
StatusCodes.Status200OK)] [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[EndpointSummary("List enrolments for a course")]

public async Task<IActionResult> GetEnrollments(
    int courseId,
    CancellationToken ct)
{
    var course = await courseService.GetByIdAsync(courseId, ct);

    if (course is null)
    {
        return NotFound();
    }

    var enrollments = await enrollmentService.GetByCourseAsync(
        courseId,
        ct);

    return Ok(enrollments);
}




}
public record CreateEnrollmentRequest(string StudentId, string CourseCode);