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
using EMS.Data.DataServices;
using EMS.Services.DataServiceFactory;
using EMS.Services.VirtualCollections;
using EMS.Tools;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.Services
{
    public class EmployeeService : IEmployeeService
    {
        public EmployeeService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<EmployeeModel> GetEmployeeAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetEmployeeAsync(dataService, id);
            }
        }
        static private async Task<EmployeeModel> GetEmployeeAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetEmployeeAsync(id);
            if (item != null)
            {
                return await CreateEmployeeModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<EmployeeModel>> GetEmployeesAsync(DataRequest<Employee> request)
        {
            var collection = new EmployeeCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<EmployeeModel>> GetEmployeesAsync(int skip, int take, DataRequest<Employee> request)
        {
            var models = new List<EmployeeModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetEmployeesAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateEmployeeModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetEmployeesCountAsync(DataRequest<Employee> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetEmployeesCountAsync(request);
            }
        }

        public async Task<int> UpdateEmployeeAsync(EmployeeModel model)
        {
            long id = model.EmployeeID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var employee = id > 0 ? await dataService.GetEmployeeAsync(model.EmployeeID) : new Employee();
                if (employee != null)
                {
                    UpdateEmployeeFromModel(employee, model);
                    await dataService.UpdateEmployeeAsync(employee);
                    model.Merge(await GetEmployeeAsync(dataService, employee.EmployeeID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteEmployeeAsync(EmployeeModel model)
        {
            var employee = new Employee { EmployeeID = model.EmployeeID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteEmployeesAsync(employee);
            }
        }

        public async Task<int> DeleteEmployeeRangeAsync(int index, int length, DataRequest<Employee> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetEmployeeKeysAsync(index, length, request);
                return await dataService.DeleteEmployeesAsync(items.ToArray());
            }
        }

        static public async Task<EmployeeModel> CreateEmployeeModelAsync(Employee source, bool includeAllFields)
        {
            var model = new EmployeeModel()
            {
                EmployeeID = source.EmployeeID,
                Title = source.Title,
                FirstName = source.FirstName,
                MiddleName = source.MiddleName,
                LastName = source.LastName,
                Suffix = source.Suffix,
                Gender = source.Gender,
                EmailAddress = source.EmailAddress,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                City = source.City,
                Region = source.Region,
                CountryCode = source.CountryCode,
                PostalCode = source.PostalCode,
                Phone = source.Phone,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail,
                ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail)
            };
            if (includeAllFields)
            {
                model.BirthDate = source.BirthDate;
                model.Education = source.Education;
                model.Occupation = source.Occupation;
                model.YearlyIncome = source.YearlyIncome;
                model.MaritalStatus = source.MaritalStatus;
                model.TotalChildren = source.TotalChildren;
                model.ChildrenAtHome = source.ChildrenAtHome;
                model.IsHouseOwner = source.IsHouseOwner;
                model.NumberCarsOwned = source.NumberCarsOwned;
                model.Picture = source.Picture;
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
            }
            return model;
        }

        private void UpdateEmployeeFromModel(Employee target, EmployeeModel source)
        {
            target.Title = source.Title;
            target.FirstName = source.FirstName;
            target.MiddleName = source.MiddleName;
            target.LastName = source.LastName;
            target.Suffix = source.Suffix;
            target.Gender = source.Gender;
            target.EmailAddress = source.EmailAddress;
            target.AddressLine1 = source.AddressLine1;
            target.AddressLine2 = source.AddressLine2;
            target.City = source.City;
            target.Region = source.Region;
            target.CountryCode = source.CountryCode;
            target.PostalCode = source.PostalCode;
            target.Phone = source.Phone;
            target.BirthDate = source.BirthDate;
            target.Education = source.Education;
            target.Occupation = source.Occupation;
            target.YearlyIncome = source.YearlyIncome;
            target.MaritalStatus = source.MaritalStatus;
            target.TotalChildren = source.TotalChildren;
            target.ChildrenAtHome = source.ChildrenAtHome;
            target.IsHouseOwner = source.IsHouseOwner;
            target.NumberCarsOwned = source.NumberCarsOwned;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }
    }
}
