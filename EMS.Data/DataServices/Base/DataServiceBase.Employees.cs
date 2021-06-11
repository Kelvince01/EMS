#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Data.Common;
using EMS.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.Data.DataServices.Base
{
    partial class DataServiceBase
    {
        public async Task<Employee> GetEmployeeAsync(long id)
        {
            return await _dataSource.Employees.Where(r => r.EmployeeID == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Employee>> GetEmployeesAsync(int skip, int take, DataRequest<Employee> request)
        {
            IQueryable<Employee> items = GetEmployees(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Employee
                {
                    EmployeeID = r.EmployeeID,
                    Title = r.Title,
                    FirstName = r.FirstName,
                    MiddleName = r.MiddleName,
                    LastName = r.LastName,
                    Suffix = r.Suffix,
                    Gender = r.Gender,
                    EmailAddress = r.EmailAddress,
                    AddressLine1 = r.AddressLine1,
                    AddressLine2 = r.AddressLine2,
                    City = r.City,
                    Region = r.Region,
                    CountryCode = r.CountryCode,
                    PostalCode = r.PostalCode,
                    Phone = r.Phone,
                    CreatedOn = r.CreatedOn,
                    LastModifiedOn = r.LastModifiedOn,
                    Thumbnail = r.Thumbnail
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Employee>> GetEmployeeKeysAsync(int skip, int take, DataRequest<Employee> request)
        {
            IQueryable<Employee> items = GetEmployees(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Employee
                {
                    EmployeeID = r.EmployeeID,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Employee> GetEmployees(DataRequest<Employee> request)
        {
            IQueryable<Employee> items = _dataSource.Employees;

            // Query
            if (!String.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order By
            if (request.OrderBy != null)
            {
                items = items.OrderBy(request.OrderBy);
            }
            if (request.OrderByDesc != null)
            {
                items = items.OrderByDescending(request.OrderByDesc);
            }

            return items;
        }

        public async Task<int> GetEmployeesCountAsync(DataRequest<Employee> request)
        {
            IQueryable<Employee> items = _dataSource.Employees;

            // Query
            if (!String.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateEmployeeAsync(Employee Employee)
        {
            if (Employee.EmployeeID > 0)
            {
                _dataSource.Entry(Employee).State = EntityState.Modified;
            }
            else
            {
                Employee.EmployeeID = UIDGenerator.Next();
                Employee.CreatedOn = DateTime.UtcNow;
                _dataSource.Entry(Employee).State = EntityState.Added;
            }
            Employee.LastModifiedOn = DateTime.UtcNow;
            Employee.SearchTerms = Employee.BuildSearchTerms();
            int res = await _dataSource.SaveChangesAsync();
            return res;
        }

        public async Task<int> DeleteEmployeesAsync(params Employee[] Employees)
        {
            _dataSource.Employees.RemoveRange(Employees);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
