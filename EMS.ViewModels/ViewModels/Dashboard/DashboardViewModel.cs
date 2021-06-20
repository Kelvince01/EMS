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
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.Customers;
using EMS.ViewModels.ViewModels.Employees;
using EMS.ViewModels.ViewModels.Orders;
using EMS.ViewModels.ViewModels.Projects;

namespace EMS.ViewModels.ViewModels.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        public DashboardViewModel(IEmployeeService employeeService, ICustomerService customerService, IOrderService orderService, IProjectService projectService, ICommonServices commonServices) : base(commonServices)
        {
            EmployeeService = employeeService;
            CustomerService = customerService;
            OrderService = orderService;
            ProjectService = projectService;
        }

        public IEmployeeService EmployeeService { get; }
        public ICustomerService CustomerService { get; }
        public IOrderService OrderService { get; }
        public IProjectService ProjectService { get; }

        private IList<EmployeeModel> _employees = null;
        public IList<EmployeeModel> Employees
        {
            get => _employees;
            set => Set(ref _employees, value);
        }

        private IList<CustomerModel> _customers = null;
        public IList<CustomerModel> Customers
        {
            get => _customers;
            set => Set(ref _customers, value);
        }

        private IList<ProjectModel> _projects = null;
        public IList<ProjectModel> Projects
        {
            get => _projects;
            set => Set(ref _projects, value);
        }

        private IList<OrderModel> _orders = null;
        public IList<OrderModel> Orders
        {
            get => _orders;
            set => Set(ref _orders, value);
        }

        public async Task LoadAsync()
        {
            StartStatusMessage("Loading dashboard...");
            await LoadEmployeesAsync();
            await LoadCustomersAsync();
            await LoadOrdersAsync();
            await LoadProjectsAsync();
            EndStatusMessage("Dashboard loaded");
        }
        public void Unload()
        {
            Employees = null;
            Customers = null;
            Projects = null;
            Orders = null;
        }

        private async Task LoadEmployeesAsync()
        {
            try
            {
                var request = new DataRequest<Employee>
                {
                    OrderByDesc = r => r.EmployeeID
                    //OrderByDesc = r => r.CreatedOn
                };
                Employees = await EmployeeService.GetEmployeesAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Employees", ex);
            }
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var request = new DataRequest<Customer>
                {
                    OrderByDesc = r => r.CustomerID
                    //OrderByDesc = r => r.CreatedOn
                };
                Customers = await CustomerService.GetCustomersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Customers", ex);
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                var request = new DataRequest<Order>
                {
                    OrderByDesc = r => r.OrderID
                    //OrderByDesc = r => r.OrderDate
                };
                Orders = await OrderService.GetOrdersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Orders", ex);
            }
        }

        private async Task LoadProjectsAsync()
        {
            try
            {
                var request = new DataRequest<Project>
                {
                    //OrderByDesc = r => r.CreatedOn
                    OrderByDesc = r => r.ProjectID
                };
                Projects = await ProjectService.GetProjectsAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Projects", ex);
            }
        }

        public void ItemSelected(string item)
        {
            switch (item)
            {
                case "Employees":
                    NavigationService.Navigate<EmployeesViewModel>(new EmployeeListArgs { OrderByDesc = r => r.EmployeeID });
                    break;
                case "Customers":
                    NavigationService.Navigate<CustomersViewModel>(new CustomerListArgs { OrderByDesc = r => r.CustomerID });
                    break;
                case "Orders":
                    NavigationService.Navigate<OrdersViewModel>(new OrderListArgs { OrderByDesc = r => r.OrderID });
                    break;
                case "Projects":
                    NavigationService.Navigate<ProjectsViewModel>(new ProjectListArgs { OrderByDesc = r => r.ProjectID });
                    break;
                default:
                    break;
            }
        }
    }
}
