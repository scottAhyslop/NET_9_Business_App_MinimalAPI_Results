using NET_9_Business_App_MinimalAPI_Results.Models;
using static NET_9_Business_App_MinimalAPI_Results.Models.EmployeesRepository;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseRouting();


//Using the default endpoint routing

//DEFAULT landing endpoint showing connection and test data
app.MapGet("/", async (HttpContext context) =>
{
    context.Response.Headers["Content-Type"] = "text/html";
    await context.Response.WriteAsync($"<h2>Test Data</h2><h3>Your page has loaded properly</h3><h4>Your endpoints are avilable for data...</h4>");
    await context.Response.WriteAsync($"The Method is: {context.Request.Method}<br/>");
    await context.Response.WriteAsync($"The URL is: {context.Request.Path}<br/>");
    await context.Response.WriteAsync($"<br/><b>Headers</b>: <br/>");
    await context.Response.WriteAsync($"<ul>");

    foreach (var key in context.Request.Headers.Keys)
    {
        await context.Response.WriteAsync($"<li><b>{key}</b>: {context.Request.Headers[key]}</li>");
    }
    await context.Response.WriteAsync($"</ul>");
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
        return Results.BadRequest("Employee is not found or is invalid...");
    }
    AddEmployee(employee);
    return TypedResults.Created($"/employees/{employee.EmployeeId}", employee);

}).WithParameterValidation();
app.Run();
