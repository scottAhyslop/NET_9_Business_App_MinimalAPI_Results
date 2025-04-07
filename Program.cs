using Microsoft.AspNetCore.Http.HttpResults;
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

app.UseStatusCodePages();//adds status code pages middleware to the pipeline, also in RFC 7807 format

//Using the default endpoint routing

//DEFAULT landing endpoint showing connection and test data
app.MapGet("/", HtmlResults()=>
{
    string html = "<h2>Test Data</h2><h3>Your page has loaded properly</h3><h4>Your endpoints are avilable for data...</h4><h4>This is a demonstration project showing different aspects of ASP.NET Core 9.0</h4>";
    
    return new HtmlResults(html);
});//End Default

//GET all employees
app.MapGet ("/employees", () =>
{
    var employees = GetEmployees();
    return TypedResults.Ok(employees);//serializes response and puts into the HTML body
});

//GET EmployeeById
app.MapGet("/employees/{id:int}", (int id) =>
{
    var employee = GetEmployeeById(id);
    return employee is not null //validate check for null or invalid employee
    ? TypedResults.Ok(employee)//if ok send it back
    : Results.ValidationProblem(new Dictionary<string, string[]>//if not snd back a validation problem msg
        {
            { "id", new[] {$"Employee with the id of {id} not provided, or does not exist."} }
        }, statusCode:404);//Not found
});

//POST /employees AddEmployee
app.MapPost("/employees", (Employee employee) =>
{
if (employee is null || employee.EmployeeId < 0)//null check on passed in param
{
    return Results.ValidationProblem(new Dictionary<string, string[]>{

        {"id", new[] { "Employee is not provided or is not valid." }   }
    }, statusCode: 400);//BadRequest
    }//end negative results RFC 7807 standard

    AddEmployee(employee);//add employee to the list
    return TypedResults.Created($"/employees/{employee.EmployeeId}", employee);//return the created employee
}).WithParameterValidation();

//PUT UpdateEmployee
app.MapPut("/employees/{id:int}", (int id, Employee employee) =>
{
     if (id != employee.EmployeeId)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        { 
           
            {"id", new[] { "EmployeeId is not valid or nonexistent..."} }

        }, statusCode: 404);//Not Found, end validation check in RFC 7807 standard
    }
   
  return UpdateEmployee(employee) ?//update the employee in the List
    TypedResults.NoContent()//else return no content
    : Results.ValidationProblem(new Dictionary<string, string[]>
    {
        {"id", new[] { "Employee does not exist." } }//report error
    }, statusCode: 404);//Bad Request, validation check in RFC 7807 standard    
}).WithParameterValidation();

//DELETE Employee
app.MapDelete("/employees/{id:int}", (int id) =>
{
    var employee = GetEmployeeById(id);

    return DeleteEmployee(employee) //delete the employee    
    ? TypedResults.Ok(employee) ://return employee Ok if deleted, else show error message
    Results.ValidationProblem(new Dictionary<string, string[]>
    { 
        {"id", new[] { "Employee is not provided or is not valid." }   }
    }, statusCode: 404);//BadRequest;//end negative results RFC 7807 standard

}).WithParameterValidation();

app.Run();