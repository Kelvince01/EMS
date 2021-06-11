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

namespace EMS.Data.DataServices
{
    public interface IDataService : IDisposable
    {
        Task<Employee> GetEmployeeAsync(long id);
        Task<IList<Employee>> GetEmployeesAsync(int skip, int take, DataRequest<Employee> request);
        Task<IList<Employee>> GetEmployeeKeysAsync(int skip, int take, DataRequest<Employee> request);
        Task<int> GetEmployeesCountAsync(DataRequest<Employee> request);
        Task<int> UpdateEmployeeAsync(Employee employee);
        Task<int> DeleteEmployeesAsync(params Employee[] employees);

        Task<Company> GetCompanyAsync(long id);
        Task<IList<Company>> GetCompanysAsync(int skip, int take, DataRequest<Company> request);
        Task<IList<Company>> GetCompanyKeysAsync(int skip, int take, DataRequest<Company> request);
        Task<int> GetCompanysCountAsync(DataRequest<Company> request);
        Task<int> UpdateCompanyAsync(Company company);
        Task<int> DeleteCompanysAsync(params Company[] companys);

        Task<Order> GetOrderAsync(long id);
        Task<IList<Order>> GetOrdersAsync(int skip, int take, DataRequest<Order> request);
        Task<IList<Order>> GetOrderKeysAsync(int skip, int take, DataRequest<Order> request);
        Task<int> GetOrdersCountAsync(DataRequest<Order> request);
        Task<int> UpdateOrderAsync(Order order);
        Task<int> DeleteOrdersAsync(params Order[] orders);

        Task<OrderItem> GetOrderItemAsync(long orderID, int orderLine);
        Task<IList<OrderItem>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request);
        Task<IList<OrderItem>> GetOrderItemKeysAsync(int skip, int take, DataRequest<OrderItem> request);
        Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request);
        Task<int> UpdateOrderItemAsync(OrderItem orderItem);
        Task<int> DeleteOrderItemsAsync(params OrderItem[] orderItems);

        Task<Project> GetProjectAsync(long id);
        Task<IList<Project>> GetProjectsAsync(int skip, int take, DataRequest<Project> request);
        Task<IList<Project>> GetProjectKeysAsync(int skip, int take, DataRequest<Project> request);
        Task<int> GetProjectsCountAsync(DataRequest<Project> request);
        Task<int> UpdateProjectAsync(Project project);
        Task<int> DeleteProjectsAsync(params Project[] projects);


        Task<IList<Category>> GetCategoriesAsync();
        Task<IList<CountryCode>> GetCountryCodesAsync();
        Task<IList<OrderStatus>> GetOrderStatusAsync();
        Task<IList<PaymentType>> GetPaymentTypesAsync();
        Task<IList<Shipper>> GetShippersAsync();
        Task<IList<TaxType>> GetTaxTypesAsync();
    }
}
