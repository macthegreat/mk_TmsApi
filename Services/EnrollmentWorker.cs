namespace MK_TmsApi.Services;
public class EnrollmentWorker(IServiceScopeFactory scopeFactory)
{
public void ProcessBatch()
    {
        using var scope = scopeFactory.CreateScope();

        var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
        
    } 
}