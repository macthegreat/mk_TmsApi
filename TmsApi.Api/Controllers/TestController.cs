using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Domain.Entities;
using TmsApi.Application.Services;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController(TmsDbContext context) : ControllerBase
{
    [HttpGet("deferred")]
    public IActionResult TestDeferred()
    {
        var query = context.Students
            .Where(s => s.GPA >= 3.0m);

        var orderedQuery = query.OrderBy(s => s.Name);

        var results = orderedQuery.ToList();

        return Ok(results);
    }

    [HttpGet("translation-fail")]
    public async Task<IActionResult> TestTranslationFail()
    {
        var students = await context.Students
            .Where(s => s.GPA >= 3.5m)
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("active-honor-count")]
    public async Task<IActionResult> ActiveHonorCount()
    {
        var count = await context.Students
            .Where(s => s.IsActive && s.GPA >= 3.0m)
            .CountAsync();

        return Ok(new
        {
            count
        });
    }

    [HttpGet("courses-by-enrollments")]
    public async Task<IActionResult> CoursesByEnrollments()
    {
        var list = await context.Courses
            .Select(c => new
            {
                c.Title,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("average-gpa-by-course")]
    public async Task<IActionResult> AverageGpaByCourse()
    {
        var list = await context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                Course = g.Key,
                AverageGPA = g.Average(e => e.Student.GPA)
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("students-with-zero-enrollments")]
    public async Task<IActionResult> StudentsWithZeroEnrollments()
    {
        var list = await context.Students
            .Where(s => !s.Enrollments.Any())
            .Select(s => s.Name)
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("students-with-zero-enrollments-leftjoin")]
    public async Task<IActionResult> StudentsWithZeroEnrollmentsLeftJoin()
    {
        var list = await context.Students
            .LeftJoin(
                context.Enrollments,
                s => s.Id,
                e => e.StudentId,
                (s, e) => new { s, e }
            )
            .Where(x => x.e == null)
            .Select(x => x.s.Name)
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(
    int page = 1,
    CancellationToken cancellationToken = default)
    {
        const int pageSize = 20;

        if (page < 1)
        {
            return BadRequest(new
            {
                Message = "Page must be greater than 0."
            });
        }

        var students = await context.Students
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(students);
    }

    [HttpGet("top-courses")]
    public async Task<IActionResult> GetTopCourses(
    CancellationToken cancellationToken = default)
    {
        var courses = await context.Enrollments
            .GroupBy(e => new
            {
                e.CourseId,
                e.Course.Title
            })
            .Select(g => new
            {
                CourseTitle = g.Key.Title,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        return Ok(courses);
    }

    //n+1 trap simulation

    [HttpGet("n-plus-one")]
public async Task<IActionResult> TestNPlusOne(
    CancellationToken cancellationToken)
{
    var students = await context.Students
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    var report = new List<object>();

    foreach (var s in students)
    {
        var count = await context.Enrollments
            .AsNoTracking()
            .CountAsync(
                e => e.StudentId == s.Id,
                cancellationToken);

        Console.WriteLine(
            $"{s.Name}: {count} enrollments"
        );

        report.Add(new
        {
            s.Name,
            EnrollmentCount = count
        });
    }

    return Ok(report);
}

//n=1 trap hundled 

[HttpGet("n-plus-one-fixed")]
public async Task<IActionResult> TestNPlusOneFixed(
    CancellationToken cancellationToken)
{
    var report = await context.Students
        .AsNoTracking()
        .Select(s => new
        {
            s.Name,
            EnrollmentCount = s.Enrollments.Count
        })
        .ToListAsync(cancellationToken);

    foreach (var r in report)
    {
        Console.WriteLine(
            $"{r.Name}: {r.EnrollmentCount} enrollments"
        );
    }

    return Ok(report);
}

[HttpPost("enrollments/archive")]
public async Task<IActionResult> ArchiveOldEnrollments(
    CancellationToken cancellationToken)
{
    var cutoff = DateTime.UtcNow.AddYears(-1);

    var affected = await context.Enrollments
        .Where(e => e.EnrolledAt < cutoff && !e.IsArchived)
        .ExecuteUpdateAsync(
            setters => setters.SetProperty(
                e => e.IsArchived,
                true),
            cancellationToken);

    return Ok(new
    {
        cutoff,
        archived = affected
    });
}
}