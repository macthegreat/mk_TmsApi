using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Dtos;
using TmsApi.Application.Services;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Services;

public class EnrollmentService(
    TmsDbContext context,
    ILogger<EnrollmentService> logger) : IEnrollmentService
{
    public Task<EnrollmentResponseDto?> GetByIdAsync(
        int courseId,
        int id,
        CancellationToken ct)
    {
        return context.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == id && e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto(
                e.Id,
                e.CourseId,
                e.StudentId,
                e.EnrolledAt))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync(
        int courseId,
        CancellationToken ct)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto(
                e.Id,
                e.CourseId,
                e.StudentId,
                e.EnrolledAt))
            .ToListAsync(ct);
    }

    public async Task<EnrollmentResponseDto> CreateAsync(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct)
    {
        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = request.StudentId,
            EnrolledAt = DateTime.UtcNow
        };

        context.Enrollments.Add(enrollment);

        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "Created enrollment {EnrollmentId} for student {StudentId} in course {CourseId}",
            enrollment.Id,
            enrollment.StudentId,
            enrollment.CourseId);

        return (await GetByIdAsync(courseId, enrollment.Id, ct))!;
    }

    public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(
    int studentId,
    CancellationToken ct)
{
    return await context.Enrollments
        .AsNoTracking()
        .Include(e => e.Course)
        .Where(e => e.StudentId == studentId)
        .ToListAsync(ct);
}
public async Task<bool> ExistsAsync(
    int studentId,
    string courseCode,
    CancellationToken ct)
{
    return await context.Enrollments
        .AnyAsync(
            e => e.StudentId == studentId &&
                 e.Course.Code == courseCode,
            ct);
}
public async Task AddAsync(
    Enrollment enrollment,
    CancellationToken ct)
{
    context.Enrollments.Add(enrollment);

    await context.SaveChangesAsync(ct);
}
}






// using Microsoft.EntityFrameworkCore;
// using TmsApi.Infrastructure.Persistence;
// using TmsApi.Application.Dtos;
// using TmsApi.Domain.Entities;


// namespace TmsApi.Infrastructure.Services;

// // public interface IEnrollmentService
// // {
// //     Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode);
// //     Task<EnrollmentRecord?> GetByIdAsync(string id);
// //     Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();
// //     Task<bool> DeleteAsync(string id);
// // }

// public class EnrollmentService(TmsDbContext context, ILogger<EnrollmentService> logger) : IEnrollmentService
// {
//     private readonly Dictionary<string, EnrollmentRecord> _store = new();

//     //private readonly ILogger<EnrollmentService> _logger;
//     public Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode)
//     {
//         var existing = _store.Values.FirstOrDefault(e => e.StudentId == studentId && e.CourseCode == courseCode);
//         if (existing is not null)
//         {
//             logger.LogWarning(
//                 "Duplicate enrollment attempt {StudentId} already in {CourseCode} (record {EnrollmentId})",
//                 studentId, courseCode, existing.Id);
//             return Task.FromResult(existing);
//         }
//         var id = Guid.NewGuid().ToString("N")[..8];
//         var record = new EnrollmentRecord(id, studentId, courseCode, DateTime.UtcNow); _store[id] = record;

//         logger.LogInformation(
//         "Enrolled {StudentId} in {CourseCode} record {EnrollmentId}", studentId, courseCode, id);
//         return Task.FromResult(record);
//     }
//     public Task<EnrollmentRecord?> GetByIdAsync(string id)
//     {

//         _store.TryGetValue(id, out var record);
//         if (record is null)
//         {
//             logger.LogWarning("Enrollment record {EnrollmentId} not found", id);
//         }
//         return Task.FromResult(record);
//     }
//     public Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync()
//     {
//         IReadOnlyList<EnrollmentRecord> all = _store.Values.ToList();
//         return Task.FromResult(all);
//     }
//     public Task<bool> DeleteAsync(string id)
//     {

//         var removed = _store.Remove(id);
//         if (removed)
//         {
//             logger.LogInformation("Enrollment record {EnrollmentId} deleted", id);
//         }
//         else
//         {
//             logger.LogWarning("Enrollment record {EnrollmentId} not found for deletion", id);
//         }
//         return Task.FromResult(removed);
//     }

//     public Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct) =>
//     context.Enrollments
//     .AsNoTracking()
//     .Where(e => e.Id == id && e.CourseId == courseId)
//     .Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.StudentId, e.EnrolledAt))
//     .FirstOrDefaultAsync(ct);

//     public async Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct)
//     {

//         var enrollment = new Enrollment
//         {
//             CourseId = courseId,
//             StudentId = request.StudentId,
//             EnrolledAt = DateTime.UtcNow
//         };
//         context.Enrollments.Add(enrollment);
//         await context.SaveChangesAsync(ct);


//         logger.LogInformation(
//         "Created enrollment {EnrollmentId} for student {StudentId} in course {CourseId}",
//         enrollment.Id,
//         enrollment.StudentId,
//         enrollment.CourseId);

//         var result = await GetByIdAsync(
//             courseId,
//             enrollment.Id,
//             ct);

//         return result!;

//     }

//     public async Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync(
//     int courseId,
//     CancellationToken ct)
//     {
//         return await context.Enrollments
//             .AsNoTracking()
//             .Where(e => e.CourseId == courseId)
//             .Select(e => new EnrollmentResponseDto(
//                 e.Id,
//                 e.CourseId,
//                 e.StudentId,
//                 e.EnrolledAt))
//             .ToListAsync(ct);
//     }

// }
 public class TmsDatabaseException(string Message) : Exception(Message);

// // --- The data shape ---


