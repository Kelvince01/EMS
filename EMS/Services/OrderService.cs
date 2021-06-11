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
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.Services
{
    public class OrderService : IOrderService
    {
        public OrderService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<OrderModel> GetOrderAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetOrderAsync(dataService, id);
            }
        }
        static private async Task<OrderModel> GetOrderAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetOrderAsync(id);
            if (item != null)
            {
                return await CreateOrderModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(DataRequest<Order> request)
        {
            var collection = new OrderCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            var models = new List<OrderModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrdersAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateOrderModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetOrdersCountAsync(request);
            }
        }

        public async Task<OrderModel> CreateNewOrderAsync(long employeeID)
        {
            var model = new OrderModel
            {
                EmployeeID = employeeID,
                OrderDate = DateTime.UtcNow,
                Status = 0
            };
            if (employeeID > 0)
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var parent = await dataService.GetEmployeeAsync(employeeID);
                    if (parent != null)
                    {
                        model.EmployeeID = employeeID;
                        model.ShipAddress = parent.AddressLine1;
                        model.ShipCity = parent.City;
                        model.ShipRegion = parent.Region;
                        model.ShipCountryCode = parent.CountryCode;
                        model.ShipPostalCode = parent.PostalCode;
                        model.Employee = await EmployeeService.CreateEmployeeModelAsync(parent, includeAllFields: true);
                    }
                }
            }
            return model;
        }

        public async Task<int> UpdateOrderAsync(OrderModel model)
        {
            long id = model.OrderID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var order = id > 0 ? await dataService.GetOrderAsync(model.OrderID) : new Order();
                if (order != null)
                {
                    UpdateOrderFromModel(order, model);
                    await dataService.UpdateOrderAsync(order);
                    model.Merge(await GetOrderAsync(dataService, order.OrderID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteOrderAsync(OrderModel model)
        {
            var order = new Order { OrderID = model.OrderID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteOrdersAsync(order);
            }
        }

        public async Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrderKeysAsync(index, length, request);
                return await dataService.DeleteOrdersAsync(items.ToArray());
            }
        }

        static public async Task<OrderModel> CreateOrderModelAsync(Order source, bool includeAllFields)
        {
            var model = new OrderModel()
            {
                OrderID = source.OrderID,
                EmployeeID = source.EmployeeID,
                OrderDate = source.OrderDate,
                ShippedDate = source.ShippedDate,
                DeliveredDate = source.DeliveredDate,
                Status = source.Status,
                PaymentType = source.PaymentType,
                TrackingNumber = source.TrackingNumber,
                ShipVia = source.ShipVia,
                ShipAddress = source.ShipAddress,
                ShipCity = source.ShipCity,
                ShipRegion = source.ShipRegion,
                ShipCountryCode = source.ShipCountryCode,
                ShipPostalCode = source.ShipPostalCode,
                ShipPhone = source.ShipPhone,
            };
            if (source.Employee != null)
            {
                model.Employee = await EmployeeService.CreateEmployeeModelAsync(source.Employee, includeAllFields);
            }
            return model;
        }

        private void UpdateOrderFromModel(Order target, OrderModel source)
        {
            target.EmployeeID = source.EmployeeID;
            target.OrderDate = source.OrderDate;
            target.ShippedDate = source.ShippedDate;
            target.DeliveredDate = source.DeliveredDate;
            target.Status = source.Status;
            target.PaymentType = source.PaymentType;
            target.TrackingNumber = source.TrackingNumber;
            target.ShipVia = source.ShipVia;
            target.ShipAddress = source.ShipAddress;
            target.ShipCity = source.ShipCity;
            target.ShipRegion = source.ShipRegion;
            target.ShipCountryCode = source.ShipCountryCode;
            target.ShipPostalCode = source.ShipPostalCode;
            target.ShipPhone = source.ShipPhone;
        }
    }
}
