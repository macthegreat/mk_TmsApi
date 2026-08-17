using TmsApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace TmsApi.Infrastructure.Services;
public class EnrollmentWorker(IServiceScopeFactory scopeFactory)
{
public void ProcessBatch()
    {
        using var scope = scopeFactory.CreateScope();

        var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
        
    } 
}