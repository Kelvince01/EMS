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
using System.Collections.Concurrent;

using Windows.UI.ViewManagement;
using EMS.Services;
using EMS.Services.DataServiceFactory;
using EMS.Services.Infrastructure;
using EMS.Services.Infrastructure.LogService;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.AppLogs;
using EMS.ViewModels.ViewModels.Charts;
using EMS.ViewModels.ViewModels.Company;
using EMS.ViewModels.ViewModels.Customers;
using EMS.ViewModels.ViewModels.Dashboard;
using EMS.ViewModels.ViewModels.Employees;
using EMS.ViewModels.ViewModels.Login;
using EMS.ViewModels.ViewModels.OrderItems;
using EMS.ViewModels.ViewModels.Orders;
using EMS.ViewModels.ViewModels.Projects;
using EMS.ViewModels.ViewModels.Reports;
using EMS.ViewModels.ViewModels.Settings;
using EMS.ViewModels.ViewModels.Shell;
using EMS.ViewModels.ViewModels.Statistics;
using Microsoft.Extensions.DependencyInjection;


namespace EMS.Configuration
{
    public class ServiceLocator : IDisposable
    {
        static private readonly ConcurrentDictionary<int, ServiceLocator> _serviceLocators = new ConcurrentDictionary<int, ServiceLocator>();

        static private ServiceProvider _rootServiceProvider = null;

        static public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISettingsService, SettingsService>();
            serviceCollection.AddSingleton<IDataServiceFactory, DataServiceFactory>();
            serviceCollection.AddSingleton<ILookupTables, LookupTables>();
            serviceCollection.AddSingleton<IEmployeeService, EmployeeService>();
            serviceCollection.AddSingleton<ICustomerService, CustomerService>();
            serviceCollection.AddSingleton<IOrderService, OrderService>();
            serviceCollection.AddSingleton<IOrderItemService, OrderItemService>();
            serviceCollection.AddSingleton<IProjectService, ProjectService>();
            serviceCollection.AddSingleton<ICompanyService, CompanyService>();

            serviceCollection.AddSingleton<IMessageService, MessageService>();
            serviceCollection.AddSingleton<ILogService, LogService>();
            serviceCollection.AddSingleton<IDialogService, DialogService>();
            serviceCollection.AddSingleton<IFilePickerService, FilePickerService>();
            serviceCollection.AddSingleton<ILoginService, LoginService>();

            serviceCollection.AddScoped<IContextService, ContextService>();
            serviceCollection.AddScoped<INavigationService, NavigationService>();
            serviceCollection.AddScoped<ICommonServices, CommonServices>();

            serviceCollection.AddTransient<LoginViewModel>();

            serviceCollection.AddTransient<ShellViewModel>();
            serviceCollection.AddTransient<MainShellViewModel>();

            serviceCollection.AddTransient<DashboardViewModel>();

            serviceCollection.AddTransient<EmployeesViewModel>();
            serviceCollection.AddTransient<EmployeeDetailsViewModel>();

            serviceCollection.AddTransient<CustomersViewModel>();
            serviceCollection.AddTransient<CustomerDetailsViewModel>();

            serviceCollection.AddTransient<OrdersViewModel>();
            serviceCollection.AddTransient<OrderDetailsViewModel>();
            serviceCollection.AddTransient<OrderDetailsWithItemsViewModel>();

            serviceCollection.AddTransient<OrderItemsViewModel>();
            serviceCollection.AddTransient<OrderItemDetailsViewModel>();

            serviceCollection.AddTransient<ProjectsViewModel>();
            serviceCollection.AddTransient<ProjectDetailsViewModel>();

            serviceCollection.AddTransient<AppLogsViewModel>();

            serviceCollection.AddTransient<CompanyViewModel>();
            serviceCollection.AddTransient<CompanyDetailsViewModel>();

            serviceCollection.AddTransient<SettingsViewModel>();
            serviceCollection.AddTransient<ValidateConnectionViewModel>();
            serviceCollection.AddTransient<CreateDatabaseViewModel>();
            
            serviceCollection.AddTransient<ChartsViewModel>();

            serviceCollection.AddTransient<ReportsViewModel>();
            serviceCollection.AddTransient<StatisticsViewModel>();

            _rootServiceProvider = serviceCollection.BuildServiceProvider();
        }

        static public ServiceLocator Current
        {
            get
            {
                int currentViewId = ApplicationView.GetForCurrentView().Id;
                return _serviceLocators.GetOrAdd(currentViewId, key => new ServiceLocator());
            }
        }

        static public void DisposeCurrent()
        {
            int currentViewId = ApplicationView.GetForCurrentView().Id;
            if (_serviceLocators.TryRemove(currentViewId, out ServiceLocator current))
            {
                current.Dispose();
            }
        }

        private IServiceScope _serviceScope = null;

        private ServiceLocator()
        {
            _serviceScope = _rootServiceProvider.CreateScope();
        }

        public T GetService<T>()
        {
            return GetService<T>(true);
        }

        public T GetService<T>(bool isRequired)
        {
            if (isRequired)
            {
                return _serviceScope.ServiceProvider.GetRequiredService<T>();
            }
            return _serviceScope.ServiceProvider.GetService<T>();
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_serviceScope != null)
                {
                    _serviceScope.Dispose();
                }
            }
        }
        #endregion
    }
}
