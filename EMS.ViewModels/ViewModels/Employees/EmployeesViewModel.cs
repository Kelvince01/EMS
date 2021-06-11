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
using System.Threading.Tasks;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.Orders;

namespace EMS.ViewModels.ViewModels.Employees
{
    public class EmployeesViewModel : ViewModelBase
    {
        public EmployeesViewModel(IEmployeeService employeeService, IOrderService orderService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            EmployeeService = employeeService;

            EmployeeList = new EmployeeListViewModel(EmployeeService, commonServices);
            EmployeeDetails = new EmployeeDetailsViewModel(EmployeeService, filePickerService, commonServices);
            EmployeeOrders = new OrderListViewModel(orderService, commonServices);
        }

        public IEmployeeService EmployeeService { get; }

        public EmployeeListViewModel EmployeeList { get; set; }
        public EmployeeDetailsViewModel EmployeeDetails { get; set; }
        public OrderListViewModel EmployeeOrders { get; set; }

        public async Task LoadAsync(EmployeeListArgs args)
        {
            await EmployeeList.LoadAsync(args);
        }
        public void Unload()
        {
            EmployeeDetails.CancelEdit();
            EmployeeList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<EmployeeListViewModel>(this, OnMessage);
            EmployeeList.Subscribe();
            EmployeeDetails.Subscribe();
            EmployeeOrders.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            EmployeeList.Unsubscribe();
            EmployeeDetails.Unsubscribe();
            EmployeeOrders.Unsubscribe();
        }

        private async void OnMessage(EmployeeListViewModel viewModel, string message, object args)
        {
            if (viewModel == EmployeeList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (EmployeeDetails.IsEditMode)
            {
                StatusReady();
                EmployeeDetails.CancelEdit();
            }
            EmployeeOrders.IsMultipleSelection = false;
            var selected = EmployeeList.SelectedItem;
            if (!EmployeeList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateOrders(selected);
                }
            }
            EmployeeDetails.Item = selected;
        }

        private async Task PopulateDetails(EmployeeModel selected)
        {
            try
            {
                var model = await EmployeeService.GetEmployeeAsync(selected.EmployeeID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Employees", "Load Details", ex);
            }
        }

        private async Task PopulateOrders(EmployeeModel selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await EmployeeOrders.LoadAsync(new OrderListArgs { EmployeeID = selectedItem.EmployeeID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                LogException("Employees", "Load Orders", ex);
            }
        }
    }
}
