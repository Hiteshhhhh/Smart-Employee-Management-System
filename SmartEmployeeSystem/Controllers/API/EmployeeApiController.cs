using Microsoft.AspNetCore.Mvc;
using SmartEmployeeSystem.Repositories;
using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Controllers.Api
{
    [ApiController]
    [Route("api/SmartEmployee")]
    public class EmployeeApiController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeApiController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // GET: api/SmartEmployee
        [HttpGet]
        public IActionResult GetAll()
        {
            var employees = _employeeRepository.GetEmployees();
            return Ok(employees);
        }

        // GET: api/employeeapi/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var employee = _employeeRepository.GetEmployeeModelById(id);
            if (employee == null)
                return NotFound("Employee not found!");
            return Ok(employee);
        }

        // DELETE: api/employeeapi/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var employee = _employeeRepository.GetEmployeeModelById(id);
            if (employee == null)
                return NotFound("Employee not found!");
            _employeeRepository.DeleteEmployee(id);
            return Ok("Deleted successfully!");
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] EmployeeModel employee)
        {
            if (employee == null)
                return BadRequest("Invalid data!");

            var existing = _employeeRepository.GetEmployeeModelById(id);
            if (existing == null)
                return NotFound("Employee not found!");

            employee.employee_id = id;
            _employeeRepository.UpdateEmployee(employee);
            return Ok("Updated successfully!");
        }
    }
}
