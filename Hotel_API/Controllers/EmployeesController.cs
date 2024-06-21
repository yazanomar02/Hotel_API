using AutoMapper;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee ,EmployeeForUpdate> repository;
        private readonly IMapper mapper;

        public EmployeesController(IRepository<Employee, EmployeeForUpdate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPost]
        public async Task<ActionResult<Employee>> CreatEmployeeAsync(EmployeeForCreate employee)
        {
            if (employee == null)
                return BadRequest();

            var newEmployee = new Employee()
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Title = employee.Title,
                StaretdDate = employee.StaretdDate,
                DOB = employee.DOB,
                HotelId = employee.HotelId,
                
            };

            var createResult = await repository.CreateAsync(newEmployee);

            if (createResult != null)
                return CreatedAtRoute("GetEmployee", new { id = newEmployee.Id }, newEmployee);

            else
                return BadRequest();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPut("{employeeId}")]
        public async Task<ActionResult<Employee>> UpdateEmployeeAsync(int employeeId, EmployeeForUpdate employee)
        {
            if (employee != null)
            {
                var newEmployee = new Employee()
                {
                    Id = employeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    Title = employee.Title,
                    StaretdDate = employee.StaretdDate,
                    DOB = employee.DOB,
                    IsDeleted = employee.IsDeleted,
                    HotelId = employee.HotelId,
                };

                var updateResult = await repository.UpdateAsync(newEmployee);

                if (updateResult != null)
                    return NoContent();

                else
                    return NotFound();
            }

            return BadRequest();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPatch("{employeeId}")]
        public async Task<ActionResult<Employee>> PartiallyUpdateEmployee(
            int employeeId, JsonPatchDocument<EmployeeForUpdate> patchDocument
            )
        {
            if (patchDocument == null)
                return BadRequest("Patch document is null.");

            var updatedEmployee = await repository.PartiallyUpdateAsync(employeeId, patchDocument);

            if (updatedEmployee == null)
                return NotFound();

            return NoContent();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpDelete("{employeeId}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int employeeId)
        {

            var deletedEmployee = await repository.DeleteAsync(employeeId);

            if (deletedEmployee == null)
            {
                return NotFound();
            }

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<List<EmployeeWithOut_Hotel_Bookings>>> GetEmployees(
            int pageNumber, int pageSize, string? keyword
            )
        {
            var (employees, paginationMetaData) = await repository.GetAllAsync(pageNumber, pageSize, keyword);

            if (employees == null)
                return NotFound();


            // إضافة رأس مخصص (Custom Header)
            // يحمل معلومات عن بيانات التصفح (pagination metadata).
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));


            var employeeWithOutHotelBookings = mapper.Map<List<EmployeeWithOut_Hotel_Bookings>>(employees);

            return Ok(employeeWithOutHotelBookings);
        }



        [HttpGet("{employeeId}", Name = "GetEmployee")]
        public async Task<ActionResult<Employee>> GetHotel(int employeeId)
        {
            var employee = await repository.GetByIdAsync(employeeId);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }
    }
}
