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
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.ViewModels.AppLogs;
using EMS.ViewModels.ViewModels.Charts;
using EMS.ViewModels.ViewModels.Company;
using EMS.ViewModels.ViewModels.Customers;
using EMS.ViewModels.ViewModels.Dashboard;
using EMS.ViewModels.ViewModels.Employees;
using EMS.ViewModels.ViewModels.Mails;
using EMS.ViewModels.ViewModels.Orders;
using EMS.ViewModels.ViewModels.Projects;
using EMS.ViewModels.ViewModels.Reports;
using EMS.ViewModels.ViewModels.Settings;
using EMS.ViewModels.ViewModels.Statistics;

namespace EMS.ViewModels.ViewModels.Shell
{
    public class MainShellViewModel : ShellViewModel
    {
        private readonly NavigationItem DashboardItem = new NavigationItem(0xE80F, "Dashboard", typeof(DashboardViewModel));
        private readonly NavigationItem EmployeesItem = new NavigationItem(0xE716, "Employees", typeof(EmployeesViewModel));
        private readonly NavigationItem CustomersItem = new NavigationItem(0xE716, "Customers", typeof(CustomersViewModel));
        private readonly NavigationItem OrdersItem = new NavigationItem(0xE8A1, "Orders", typeof(OrdersViewModel));
        private readonly NavigationItem ProjectsItem = new NavigationItem(0xE781, "Projects", typeof(ProjectsViewModel));
        private readonly NavigationItem AppLogsItem = new NavigationItem(0xE7BA, "Activity Log", typeof(AppLogsViewModel));
        private readonly NavigationItem SettingsItem = new NavigationItem(0x2699, "Settings", typeof(SettingsViewModel));
        private readonly NavigationItem MailsItem = new NavigationItem(0x1F4E7, "Mails", typeof(MailsViewModel));
        private readonly NavigationItem CompanyItem = new NavigationItem(0x0000, "Company", typeof(CompanyViewModel));
        private readonly NavigationItem ChartsItem = new NavigationItem(0x1F5E0, "Charts", typeof(ChartsViewModel));
        private readonly NavigationItem ReportsItem = new NavigationItem(0x0000, "Reports", typeof(ReportsViewModel));
        private readonly NavigationItem StatisticsItem = new NavigationItem(0x0000, "Statistics", typeof(StatisticsViewModel));

        public MainShellViewModel(ILoginService loginService, ICommonServices commonServices) : base(loginService, commonServices)
        {
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        private bool _isPaneOpen = true;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => Set(ref _isPaneOpen, value);
        }

        private IEnumerable<NavigationItem> _items;
        public IEnumerable<NavigationItem> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public override async Task LoadAsync(ShellArgs args)
        {
            Items =  GetItems().ToArray();
            await UpdateAppLogBadge();
            await base.LoadAsync(args);
        }

        override public void Subscribe()
        {
            MessageService.Subscribe<ILogService, AppLog>(this, OnLogServiceMessage);
            base.Subscribe();
        }

        override public void Unsubscribe()
        {
            base.Unsubscribe();
        }

        public override void Unload()
        {
            base.Unload();
        }

        public async void NavigateTo(Type viewModel)
        {
            switch (viewModel.Name)
            {
                case "DashboardViewModel":
                    NavigationService.Navigate(viewModel);
                    break;
                case "EmployeesViewModel":
                    NavigationService.Navigate(viewModel, new EmployeeListArgs());
                    break;
                case "CustomersViewModel":
                    NavigationService.Navigate(viewModel, new CustomerListArgs());
                    break;
                case "OrdersViewModel":
                    NavigationService.Navigate(viewModel, new OrderListArgs());
                    break;
                case "ProjectsViewModel":
                    NavigationService.Navigate(viewModel, new ProjectListArgs());
                    break;
                case "MailsViewModel":
                    NavigationService.Navigate(viewModel, new MailListArgs());
                    break;
                case "CompanyViewModel":
                    NavigationService.Navigate(viewModel, new CompanyListArgs());
                    break;
                case "AppLogsViewModel":
                    NavigationService.Navigate(viewModel, new AppLogListArgs());
                    await LogService.MarkAllAsReadAsync();
                    await UpdateAppLogBadge();
                    break;
                case "SettingsViewModel":
                    NavigationService.Navigate(viewModel, new SettingsArgs());
                    break;
                case "ChartsViewModel":
                    NavigationService.Navigate(viewModel, new ChartsArgs());
                    break;
                case "ReportsViewModel":
                    NavigationService.Navigate(viewModel, new ReportsArgs());
                    break;
                case "StatisticsViewModel":
                    NavigationService.Navigate(viewModel, new StatisticsArgs());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<NavigationItem> GetItems()
        {
            yield return DashboardItem;
            yield return EmployeesItem;
            yield return CustomersItem;
            yield return OrdersItem;
            yield return ProjectsItem;
            yield return AppLogsItem;
            yield return MailsItem;
            yield return CompanyItem;
            yield return ChartsItem;
            yield return ReportsItem;
            yield return StatisticsItem;
        }

        private async void OnLogServiceMessage(ILogService logService, string message, AppLog log)
        {
            if (message == "LogAdded")
            {
                await ContextService.RunAsync(async () =>
                {
                    await UpdateAppLogBadge();
                });
            }
        }

        private async Task UpdateAppLogBadge()
        {
            int count = await LogService.GetLogsCountAsync(new DataRequest<AppLog> { Where = r => !r.IsRead });
            AppLogsItem.Badge = count > 0 ? count.ToString() : null;
        }
    }
}
