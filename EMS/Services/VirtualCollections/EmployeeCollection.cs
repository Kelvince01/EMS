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
using EMS.Common.VirtualCollection;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.Services.VirtualCollections
{
    public class EmployeeCollection : VirtualCollection<EmployeeModel>
    {
        private DataRequest<Employee> _dataRequest = null;

        public EmployeeCollection(IEmployeeService employeeService, ILogService logService) : base(logService)
        {
            EmployeeService = employeeService;
        }

        public IEmployeeService EmployeeService { get; }

        private EmployeeModel _defaultItem = EmployeeModel.CreateEmpty();
        protected override EmployeeModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<Employee> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await EmployeeService.GetEmployeesCountAsync(_dataRequest);
                Ranges[0] = await EmployeeService.GetEmployeesAsync(0, RangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<EmployeeModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await EmployeeService.GetEmployeesAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                LogException("EmployeeCollection", "Fetch", ex);
            }
            return null;
        }
    }
}
