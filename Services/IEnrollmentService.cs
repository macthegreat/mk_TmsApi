using MK_TmsApi.Dtos;

namespace MK_TmsApi.Services;

public interface IEnrollmentService
{

    Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode);
    Task<EnrollmentRecord?> GetByIdAsync(string id);
    Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();
    Task<bool> DeleteAsync(string id);

    Task<EnrollmentResponseDto?> GetByIdAsync(int courseId,int id, CancellationToken ct);
    Task<EnrollmentResponseDto> CreateAsync(int courseId,EnrollStudentRequest request,CancellationToken ct);
}
