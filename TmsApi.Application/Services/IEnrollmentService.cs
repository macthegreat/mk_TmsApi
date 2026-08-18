using TmsApi.Application.Dtos;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDto?> GetByIdAsync(
        int courseId,
        int id,
        CancellationToken ct);

    Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync(
        int courseId,
        CancellationToken ct);

    Task<EnrollmentResponseDto> CreateAsync(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct);

    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(
        int studentId,
        CancellationToken ct);

    Task<bool> ExistsAsync(
        int studentId,
        string courseCode,
        CancellationToken ct);

    Task AddAsync(
        Enrollment enrollment,
        CancellationToken ct);
}


// using TmsApi.Application.Dtos;
// using TmsApi.Domain.Entities;

// namespace TmsApi.Application.Services;

// public interface IEnrollmentService
// {

//     Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode);
//     Task<EnrollmentRecord?> GetByIdAsync(string id);
//     Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();
//     Task<bool> DeleteAsync(string id);

//     Task<EnrollmentResponseDto?> GetByIdAsync(int courseId,int id, CancellationToken ct);
//     Task<EnrollmentResponseDto> CreateAsync(int courseId,EnrollStudentRequest request,CancellationToken ct);

//      Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync(
//     int courseId,
//     CancellationToken ct);
// }
