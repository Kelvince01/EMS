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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using EMS.ViewModels;
using EMS.ViewModels.ViewModels.Employees;

namespace EMS.Views.Employees.Details
{
    public sealed partial class EmployeesDetails : UserControl
    {
        public EmployeesDetails()
        {
            InitializeComponent();
        }

        #region ViewModel
        public EmployeeDetailsViewModel ViewModel
        {
            get { return (EmployeeDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(EmployeeDetailsViewModel), typeof(EmployeesDetails), new PropertyMetadata(null));
        #endregion
    }
}
