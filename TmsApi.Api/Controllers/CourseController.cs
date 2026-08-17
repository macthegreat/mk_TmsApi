using Microsoft.AspNetCore.Mvc;
using TmsApi.Domain.Entities;
using TmsApi.Application.Services;
using TmsApi.Application.Dtos;



namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]

public class CourseController(ICourseService courseService, LinkGenerator linkGenerator) : ControllerBase
{
    [HttpGet("{id:int}", Name = nameof(GetCourseById))]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a course by ID")]
    [EndpointDescription("Returns course details with HATEOAS links. Re turns 404 if the course does not exist.")]
    public async Task<IActionResult> GetCourseById(
      int id,
      CancellationToken ct)
    {
        // 1. Get course
        var course = await courseService.GetByIdAsync(id, ct);

        // 2. Return 404 if course doesn't exist
        if (course is null)
            return NotFound();

        // 3. Build route paths

        var selfPath = linkGenerator.GetPathByName(
            HttpContext,
            nameof(GetCourseById),
            new { id }
        );

        var enrollmentsPath = linkGenerator.GetPathByName(
            HttpContext,
            "ListCourseEnrollments",
            new { courseId = id }
        );

        // Use the ACTUAL names of your PUT and DELETE actions here.
        var updatePath = linkGenerator.GetPathByName(
          HttpContext,
          "UpdateCourse",
          new { id }
      );

        var deletePath = linkGenerator.GetPathByName(
            HttpContext,
            "DeleteCourse",
            new { id }
        );

        // Route names are controlled by us, so null means
        // the route configuration needs to be fixed.
        ArgumentNullException.ThrowIfNull(selfPath);
        //ArgumentNullException.ThrowIfNull(updatePath);
        //ArgumentNullException.ThrowIfNull(deletePath);
        ArgumentNullException.ThrowIfNull(enrollmentsPath);

        // 4. Build links
        var links = new List<LinkDto>
    {
        new LinkDto("self", "GET", selfPath),
        new LinkDto("update", "PUT", updatePath),
        new LinkDto("delete", "DELETE", deletePath),
        new LinkDto("enrollments", "GET", enrollmentsPath)
    };

        // 5. Add enroll link only when course has capacity
        if (course.Enrollments.Count < course.MaxCapacity)
        {
            links.Add(
                new LinkDto("enroll", "POST", enrollmentsPath)
            );
        }

        // 6. Build CourseDetailDto
        var detailDto = new CourseDetailDto
        {
            Id = course.Id,
            Code = course.Code,
            Title = course.Title,
            MaxCapacity = course.MaxCapacity,
            EnrollmentCount = course.Enrollments.Count,
            Links = links
        };

        // 7. Return response
        return Ok(detailDto);
    }
    [HttpPost]
    [ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Create a new course")]
    [EndpointDescription("Creates a course with a unique code. Returns 409 if the course code already exists.")]
    public async Task<IActionResult> CreateCourse(CreateCourseRequest request, CancellationToken ct)
    {

        if (await courseService.CodeExistsAsync(request.Code, ct))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course code already exists",
                Detail = $"A course with code '{request.Code}' is already registered.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var result = await courseService.CreateAsync(request, ct);

        return CreatedAtAction(nameof(GetCourseById),
            new { id = result.Id },
            result);

    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
    [EndpointSummary("List courses with pagination")]
    [EndpointDescription("Returns a paginated, optionally filtered list of TMS courses. PageSize is capped at 50.")]


    public async Task<IActionResult> GetCourses(
    [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await courseService.GetCoursesAsync(request, ct);
        return Ok(result);
    }






}
