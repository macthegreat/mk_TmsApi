using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MK_TmsApi.Entities;
using MK_TmsApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<EnrollmentWorker>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddOptions<PaymentOptions>().BindConfiguration("payments").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddDbContext<TmsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase")));

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages();

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
    return Results.Ok("processed");
});

app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException("Simulated database failure for ProblemDetails testing");
});

app.Run();
