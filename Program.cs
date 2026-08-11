var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer");
builder.Services.AddAuthorization();
builder.Services.AddSingleton<EnrollmentWorker>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddOptions<PaymentOptions>().BindConfiguration("payments").ValidateDataAnnotations().ValidateOnStart();

builder.Host.UseDefaultServiceProvider(options =>
{
options.ValidateScopes = true;
options.ValidateOnBuild = true; });


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();



app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS101",
    studentId = "S-001",
    letterGrade = "A",
})).RequireAuthorization();


app.MapGet("/api/enrollments/worker-smoke", (EnrollmentWorker worker) =>
{
worker.ProcessBatch();
return Results.Ok("processed"); });



app.Run();
