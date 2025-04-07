
namespace NET_9_Business_App_MinimalAPI_Results.Models
{
    public interface IEmployeesRepository
    {
        void AddEmployee(Employee employee);
        bool DeleteEmployee(Employee? employee);
        Employee? GetEmployeeById(int employeeId);
        List<Employee> GetEmployees();
        bool UpdateEmployee(Employee? employee);
    }
}