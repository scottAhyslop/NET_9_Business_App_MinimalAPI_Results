using NET_9_Business_App_MinimalAPI_Results.Models;
using NET_9_Business_App_MinimalAPI_Results.Results;
using static NET_9_Business_App_MinimalAPI_Results.Models.EmployeesRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();//adds the problem details middleware to the pipeline

var app = builder.Build();

if (!app.Environment.IsDevelopment())//always place first in middleware pipeline for all to use
{
    app.UseExceptionHandler();//defaults to RFC 7807 standard as a JSON obj output with ProblemDetails service
}

app.UseStatusCodePages();//adds status code pages middleware to the pipeline, also in 7807 format

//Using the default endpoint routing

//DEFAULT landing endpoint showing connection and test data
app.MapGet("/", HtmlResults()=>
{
    string html = "<h2>Test Data</h2><h3>Your page has loaded properly</h3><h4>Your endpoints are avilable for data...</h4><h4>This is a demonstration project showing different aspects of ASP.NET Core 9.0</h4>";
    
    return new HtmlResults(html);
});//End Default

app.MapGet ("/employees", () =>
{
    var employees = GetEmployees();
    return TypedResults.Ok(employees);//serializes response and puts into the HTML body
});
//POST /employees AddEmployee
app.MapPost("/employees", (Employee employee) =>
{
    if (employee is null || employee.EmployeeId <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
           {
            {"id", new[]{"Employee is not provided or is not valid."} }
           }           
          );
    }
    AddEmployee(employee);
    return TypedResults.Created($"/employees/{employee.EmployeeId}", employee);

}).WithParameterValidation();
app.Run();
