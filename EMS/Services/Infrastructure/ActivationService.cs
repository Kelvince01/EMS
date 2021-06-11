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

using Windows.ApplicationModel.Activation;
using EMS.Extensions;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.ViewModels.Company;
using EMS.ViewModels.ViewModels.Dashboard;
using EMS.ViewModels.ViewModels.Employees;
using EMS.ViewModels.ViewModels.Orders;
using EMS.ViewModels.ViewModels.Projects;

namespace EMS.Services.Infrastructure
{
    #region ActivationInfo
    public class ActivationInfo
    {
        static public ActivationInfo CreateDefault() => Create<DashboardViewModel>();

        static public ActivationInfo Create<TViewModel>(object entryArgs = null) where TViewModel : ViewModelBase
        {
            return new ActivationInfo
            {
                EntryViewModel = typeof(TViewModel),
                EntryArgs = entryArgs
            };
        }

        public Type EntryViewModel { get; set; }
        public object EntryArgs { get; set; }
    }
    #endregion

    static public class ActivationService
    {
        static public ActivationInfo GetActivationInfo(IActivatedEventArgs args)
        {
            switch (args.Kind)
            {
                case ActivationKind.Protocol:
                    return GetProtocolActivationInfo(args as ProtocolActivatedEventArgs);

                case ActivationKind.Launch:
                default:
                    return ActivationInfo.CreateDefault();
            }
        }

        private static ActivationInfo GetProtocolActivationInfo(ProtocolActivatedEventArgs args)
        {
            if (args != null)
            {
                switch (args.Uri.AbsolutePath.ToLowerInvariant())
                {
                    case "Employee":
                    case "Employees":
                        long employeeID = args.Uri.GetInt64Parameter("id");
                        if (employeeID > 0)
                        {
                            return ActivationInfo.Create<EmployeeDetailsViewModel>(new EmployeeDetailsArgs { EmployeeID = employeeID });
                        }
                        return ActivationInfo.Create<EmployeesViewModel>(new EmployeeListArgs());
                    case "order":
                    case "orders":
                        long orderID = args.Uri.GetInt64Parameter("id");
                        if (orderID > 0)
                        {
                            return ActivationInfo.Create<OrderDetailsViewModel>(new OrderDetailsArgs { OrderID = orderID });
                        }
                        return ActivationInfo.Create<OrdersViewModel>(new OrderListArgs());
                    case "Project":
                    case "Projects":
                        long projectID = args.Uri.GetInt64Parameter("id");
                        if (projectID > 0)
                        {
                            return ActivationInfo.Create<ProjectDetailsViewModel>(new ProjectDetailsArgs { ProjectID = projectID });
                        }
                        return ActivationInfo.Create<ProjectsViewModel>(new ProjectListArgs());
                    case "Company":
                    case "Companys":
                        long companyID = args.Uri.GetInt64Parameter("id");
                        if (companyID > 0)
                        {
                            return ActivationInfo.Create<CompanyDetailsViewModel>(new CompanyDetailsArgs { CompanyID = companyID });
                        }
                        return ActivationInfo.Create<CompanyViewModel>(new CompanyListArgs());
                }
            }
            return ActivationInfo.CreateDefault();
        }
    }
}
