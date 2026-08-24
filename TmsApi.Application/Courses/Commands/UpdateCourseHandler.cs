using MediatR;
using TmsApi.Application.Services;

namespace TmsApi.Application.Courses.Commands;

public class UpdateCourseHandler(
    ICourseService service,
    ICachedCourseService cachedService)
    : IRequestHandler<UpdateCourseCommand, bool>
{
    public async Task<bool> Handle(
        UpdateCourseCommand command,
        CancellationToken ct)
    {
        var updated = await service.UpdateAsync(command, ct);

        if (!updated)
        {
            return false;
        }

        await cachedService.InvalidateCourseCacheAsync(ct);

        return true;
    }
}