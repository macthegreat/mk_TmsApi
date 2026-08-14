using Microsoft.EntityFrameworkCore;
using MK_TmsApi.Data;
using MK_TmsApi.Entities;
using MK_TmsApi.Dtos;

namespace MK_TmsApi.Services;


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
     public async Task<bool> CodeExistsAsync(
        string code,
        CancellationToken ct)
    {
        return await context.Courses
            .AnyAsync(c => c.Code == code, ct);
    }

}