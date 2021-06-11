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
using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.ViewModels.Models;

namespace EMS.ViewModels.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeModel> GetEmployeeAsync(long id);
        Task<IList<EmployeeModel>> GetEmployeesAsync(DataRequest<Employee> request);
        Task<IList<EmployeeModel>> GetEmployeesAsync(int skip, int take, DataRequest<Employee> request);
        Task<int> GetEmployeesCountAsync(DataRequest<Employee> request);

        Task<int> UpdateEmployeeAsync(EmployeeModel model);

        Task<int> DeleteEmployeeAsync(EmployeeModel model);
        Task<int> DeleteEmployeeRangeAsync(int index, int length, DataRequest<Employee> request);
    }
}
