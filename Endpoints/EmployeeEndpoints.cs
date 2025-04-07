using NET_9_Business_App_MinimalAPI_Results.Models;
using NET_9_Business_App_MinimalAPI_Results.Results;

namespace NET_9_Business_App_MinimalAPI_Results.Endpoints
{
    public static class EmployeeEndpoints
    {
      
        public static void MapEmployeeEndpoints(this WebApplication app)
        {
            //DEFAULT landing endpoint showing connection and test data
            app.MapGet("/", HtmlResults () =>
            {
                string html = "<h2>Test Data</h2><h3>Your page has loaded properly</h3><h4>Your endpoints are avilable for data...</h4><h4>This is a demonstration project showing different aspects of ASP.NET Core 9.0</h4>";

                return new HtmlResults(html);
            });//End Default

            //GET /employees get all employees
            app.MapGet("/employees", (IEmployeesRepository employeesRepository) =>
            {
                var employees = employeesRepository.GetEmployees();
                return TypedResults.Ok(employees);//serializes response and puts into the HTML body
            });

            //GET /employees/id EmployeeById
            app.MapGet("/employees/{id:int}", (int id, IEmployeesRepository employeesRepository) =>
            {
                var employee = employeesRepository.GetEmployeeById(id);
                return employee is not null //validate check for null or invalid employee
                ? TypedResults.Ok(employee)//if ok send it back
                : Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>//if not snd back a validation problem msg
                    {
                        { "id", new[] {$"Employee with the id of {id} not provided, or does not exist."} }
                    }, statusCode: 404);//Not found
            });

            //POST /employees AddEmployee
            app.MapPost("/employees", (Employee employee, IEmployeesRepository employeesRepository) =>
            {
                if (employee is null || employee.EmployeeId < 0)//null check on passed in param
                {
                    return Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>{
                         {
                            "id", new[] { "EmployeeId is not provided or is not valid." }//Return error message
                         }
                    }, statusCode: 400);//BadRequest//provide HTML response code 
                }//end negative results RFC 7807 standard

                employeesRepository.AddEmployee(employee);//add employee to the list
                return TypedResults.Created($"/employees/{employee.EmployeeId}", employee);//return the created employee
            }).WithParameterValidation();

            //PUT /employees/id UpdateEmployee
            app.MapPut("/employees/{id:int}", (int id, Employee employee, IEmployeesRepository employeesRepository) =>
            {
                if (id != employee.EmployeeId)
                {
                    return Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {"id", new[] { "EmployeeId is not valid or nonexistent..."} }
                    }, statusCode: 404);//Not Found, end validation check in RFC 7807 standard
                }

                return employeesRepository.UpdateEmployee(employee) ?//update the employee in the List
                  TypedResults.NoContent()//else return no content
                  : Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                  {
                    {"id", new[] { "Employee does not exist." } }//report error
                  }, statusCode: 404);//Bad Request, validation check in RFC 7807 standard    
            }).WithParameterValidation();

            //DELETE /employees/id DeleteEmployee
            app.MapDelete("/employees/{id:int}", (int id, IEmployeesRepository employeesRepository) =>
            {
                var employee = employeesRepository.GetEmployeeById(id);

                return employeesRepository.DeleteEmployee(employee) //delete the employee    
                ? TypedResults.Ok(employee) ://return employee Ok if deleted, else show error message
                Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    {"id", new[] { "Employee is not provided or is not valid." }   }
                }, statusCode: 404);//BadRequest;//end negative results RFC 7807 standard

            }).WithParameterValidation();

        }//end MapEmployeeEndpoints class
    }//end EmployeeEndpoints class
}//end namespace
