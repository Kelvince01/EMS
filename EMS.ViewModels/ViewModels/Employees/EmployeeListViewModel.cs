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
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.ViewModels.Infrastructure.Common;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.Common;

namespace EMS.ViewModels.ViewModels.Employees
{
    #region EmployeeListArgs
    public class EmployeeListArgs
    {
        static public EmployeeListArgs CreateEmpty() => new EmployeeListArgs { IsEmpty = true };

        public EmployeeListArgs()
        {
            OrderBy = r => r.EmployeeID;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<Employee, object>> OrderBy { get; set; }
        public Expression<Func<Employee, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class EmployeeListViewModel : GenericListViewModel<EmployeeModel>
    {
        public EmployeeListViewModel(IEmployeeService employeeService, ICommonServices commonServices) : base(commonServices)
        {
            EmployeeService = employeeService;
        }

        public IEmployeeService EmployeeService { get; }

        public EmployeeListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(EmployeeListArgs args)
        {
            ViewModelArgs = args ?? EmployeeListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading Employees...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Employees loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<EmployeeListViewModel>(this, OnMessage);
            MessageService.Subscribe<EmployeeDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public EmployeeListArgs CreateArgs()
        {
            return new EmployeeListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;

            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            try
            {
                Items = await GetItemsAsync();
            }
            catch (Exception ex)
            {
                Items = new List<EmployeeModel>();
                StatusError($"Error loading Employees: {ex.Message}");
                LogException("Employees", "Refresh", ex);
                isOk = false;
            }

            ItemsCount = Items.Count;
            if (!IsMultipleSelection)
            {
                SelectedItem = Items.FirstOrDefault();
            }
            NotifyPropertyChanged(nameof(Title));

            return isOk;
        }

        private async Task<IList<EmployeeModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Employee> request = BuildDataRequest();
                return await EmployeeService.GetEmployeesAsync(request);
            }
            return new List<EmployeeModel>();
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await NavigationService.CreateNewViewAsync<EmployeeDetailsViewModel>(new EmployeeDetailsArgs { EmployeeID = SelectedItem.EmployeeID });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<EmployeeDetailsViewModel>(new EmployeeDetailsArgs());
            }
            else
            {
                NavigationService.Navigate<EmployeeDetailsViewModel>(new EmployeeDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading Employees...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Employees loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected Employees?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} Employees...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} Employees...");
                        await DeleteItemsAsync(SelectedItems);
                        MessageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Employees: {ex.Message}");
                    LogException("Employees", "Delete", ex);
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} Employees deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<EmployeeModel> models)
        {
            foreach (var model in models)
            {
                await EmployeeService.DeleteEmployeeAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Employee> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await EmployeeService.DeleteEmployeeRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Employee> BuildDataRequest()
        {
            return new DataRequest<Employee>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        private async void OnMessage(ViewModelBase sender, string message, object args)
        {
            switch (message)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await ContextService.RunAsync(async () =>
                    {
                        await RefreshAsync();
                    });
                    break;
            }
        }
    }
}
