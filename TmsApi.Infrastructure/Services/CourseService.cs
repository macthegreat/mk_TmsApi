using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Domain.Entities;
using TmsApi.Application.Dtos;
using TmsApi.Application.Services;

namespace TmsApi.Infrastructure.Services;


public class CourseService(
    TmsDbContext context,
    ILogger<CourseService> logger) : ICourseService
{
    public async Task<Course?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        return await context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Course> CreateAsync(
        CreateCourseRequest request,
        CancellationToken ct)
    {
        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "Created course {CourseId} with code {CourseCode}",
            course.Id,
            course.Code);

        return course;
    }

    public async Task<Course?> GetByCodeAsync(
    string code,
    CancellationToken ct)
{
    return await context.Courses
        .AsNoTracking()
        .Include(c => c.Enrollments)
        .FirstOrDefaultAsync(c => c.Code == code, ct);
}
     public async Task<bool> CodeExistsAsync(
        string code,
        CancellationToken ct)
    {
        return await context.Courses
            .AnyAsync(c => c.Code == code, ct);
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync(
    CancellationToken ct)
{
    return await context.Courses
        .AsNoTracking()
        .Include(c => c.Enrollments)
        .OrderBy(c => c.Title)
        .ThenBy(c => c.Id)
        .ToListAsync(ct);
}
  

//m6s2part c
public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
    PagedRequest request,
    CancellationToken ct)
{
    var query = context.Courses
        .AsNoTracking();

    if (!string.IsNullOrWhiteSpace(request.Search))
    {
        var search = request.Search.Trim();

        query = query.Where(c =>
            EF.Functions.ILike(c.Title, $"%{search}%") ||
            EF.Functions.ILike(c.Code, $"%{search}%"));
    }

    var totalCount = await query.CountAsync(ct);

    IQueryable<Course> orderedQuery;

    if (request.Descending)
    {
        orderedQuery = query
            .OrderByDescending(c => c.Title)
            .ThenByDescending(c => c.Id);
    }
    else
    {
        orderedQuery = query
            .OrderBy(c => c.Title)
            .ThenBy(c => c.Id);
    }

    var items = await orderedQuery
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(c => new CourseResponseDto(
            c.Id,
            c.Code,
            c.Title,
            c.MaxCapacity,
            c.Enrollments.Count))
        .ToListAsync(ct);

    return new PagedResponse<CourseResponseDto>
    {
        Items = items,
        TotalCount = totalCount,
        Page = request.Page,
        PageSize = request.PageSize
    };
}
}
