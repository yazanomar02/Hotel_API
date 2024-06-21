using Data;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class RepositoryEmployee : IRepository<Employee, EmployeeForUpdate>
    {
        private readonly HotelContext context;

        public RepositoryEmployee(HotelContext context)
        {
            this.context = context;
        }
        public async Task<Employee> CreateAsync(Employee employee)
        {
            var existingHotel = await context.Hotels
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(h => h.Id == employee.HotelId && !h.IsDeleted);

            if (existingHotel == null)
            {
                await Console.Out.WriteLineAsync("The employee hotel does not exist !!");
                return null;
            }

            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();

            await Console.Out.WriteLineAsync("Employee added successfully.");

            return employee;
        }



        public async Task<Employee> UpdateAsync(Employee employee)
        {
            // التحقق من وجود الموظف في قاعدة البيانات
            var existingEmployee = await context.Employees
                                      .FirstOrDefaultAsync(e => e.Id == employee.Id && !e.IsDeleted);
            if (existingEmployee == null)
            {
                await Console.Out.WriteLineAsync("The employee does not found !!");
                return null;
            }

            // التحقق من وجود فندق الموظف
            if (employee.HotelId != null)
            {
                // التحقق من وجود هذا الفندق في قاعدة البيانات
                var existingHotel = await context.Hotels
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(h => h.Id == employee.HotelId && !h.IsDeleted);
                if (existingHotel == null)
                {
                    Console.WriteLine("The employee's hotel does not found !!");
                    return null;
                }
            }

            // تحديث المعلومات
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.DOB = employee.DOB;
            existingEmployee.Email = employee.Email;
            existingEmployee.StaretdDate = employee.StaretdDate;
            existingEmployee.Title = employee.Title;
            existingEmployee.IsDeleted = employee.IsDeleted;
            existingEmployee.HotelId = employee.HotelId;

            context.Employees.Update(existingEmployee);
            await context.SaveChangesAsync();

            await Console.Out.WriteLineAsync("Employee updated successfully.");

            return existingEmployee;
        }



        public async Task<Employee> PartiallyUpdateAsync(int EmployeeId, JsonPatchDocument<EmployeeForUpdate> patchDocument)
        {
            var existingEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Id == EmployeeId);

            if (existingEmployee == null)
                return null;

            var employeeToPatch = new EmployeeForUpdate
            {
                FirstName = existingEmployee.FirstName,
                LastName = existingEmployee.LastName,
                Email = existingEmployee.Email,
                Title = existingEmployee.Title,
                StaretdDate = existingEmployee.StaretdDate,
                DOB = existingEmployee.DOB,
                IsDeleted = existingEmployee.IsDeleted,
                HotelId = existingEmployee.HotelId
            };

            try
            {
                // تطبيق التعديلات على الكائن
                patchDocument.ApplyTo(employeeToPatch);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"An error occurred while applying the patch document: {ex}");
                return null;
            }

            var checkHotel = await context.Hotels
                .FirstOrDefaultAsync(h => h.Id == employeeToPatch.HotelId && !h.IsDeleted);

            if (checkHotel == null)
            {
                await Console.Out.WriteLineAsync("The new hotel is not available");
                return null;
            }

            existingEmployee.FirstName = employeeToPatch.FirstName;
            existingEmployee.LastName = employeeToPatch.LastName;
            existingEmployee.StaretdDate = employeeToPatch.StaretdDate;
            existingEmployee.IsDeleted = employeeToPatch.IsDeleted;
            existingEmployee.DOB = employeeToPatch.DOB;
            existingEmployee.Email = employeeToPatch.Email;
            existingEmployee.HotelId = employeeToPatch.HotelId;
            existingEmployee.Title = employeeToPatch.Title;

            await context.SaveChangesAsync();

            return existingEmployee;
        }




        public async Task<Employee> DeleteAsync(int employeeId)
        {
            var existingEmployee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

            if (existingEmployee == null)
                return null;

            existingEmployee.IsDeleted = true;

            context.Employees.Update(existingEmployee);
            context.SaveChanges();

            return existingEmployee;
        }



        public async Task<(List<Employee>, PaginationMetaData)> GetAllAsync(
            int pageNumber, int pageSize, string keyword = null
            )
        {
            // حساب عدد الغرف الكلّي
            var totalItemCount = await context.Employees.CountAsync();

            // نمط تقسيم صفحات العرض
            var paginationData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // ضبط عرض التقسيم للصفحات
            var query = context.Employees as IQueryable<Employee>;

            if (!keyword.IsNullOrEmpty())
                query = query.Where(e => e.Title.ToLower().Contains(keyword.ToLower()));

            var employees = await query.OrderBy(p => p.Title)
                                       .Skip(pageSize * (pageNumber - 1))
                                       .Take(pageSize)
                                       .ToListAsync();

            return (employees, paginationData);
        }



        public async Task<Employee> GetByIdAsync(int employeeId)
        {
            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                return null;

            return employee;
        }
    }
}
