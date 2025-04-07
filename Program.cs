using NET_9_Business_App_MinimalAPI_Results.Endpoints;
using NET_9_Business_App_MinimalAPI_Results.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();//adds the problem details middleware to the pipeline

builder.Services.AddTransient<IEmployeesRepository, EmployeesRepository>();//register the employees repository with the DI container

var app = builder.Build();

if (!app.Environment.IsDevelopment())//always place first in middleware pipeline for all to use
{
    app.UseExceptionHandler();//defaults to RFC 7807 standard as a JSON obj output with ProblemDetails service
}

app.UseStatusCodePages();//adds status code pages middleware to the pipeline, also in RFC 7807 format

app.MapEmployeeEndpoints();//adds the employee endpoints to the pipeline

app.Run();