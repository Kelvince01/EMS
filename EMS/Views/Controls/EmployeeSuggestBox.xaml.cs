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
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;
using EMS.Configuration;
using EMS.Controls.Forms;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.Extensions;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.Views.Controls
{
    public sealed partial class EmployeeSuggestBox : UserControl
    {
        public EmployeeSuggestBox()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                EmployeeService = ServiceLocator.Current.GetService<IEmployeeService>();
            }
            InitializeComponent();
        }

        private IEmployeeService EmployeeService { get; }

        #region Items
        public IList<EmployeeModel> Items
        {
            get { return (IList<EmployeeModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<EmployeeModel>), typeof(EmployeeSuggestBox), new PropertyMetadata(null));
        #endregion

        #region DisplayText
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(EmployeeSuggestBox), new PropertyMetadata(null));
        #endregion

        #region IsReadOnly*
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EmployeeSuggestBox;
            control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(EmployeeSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
        #endregion

        #region EmployeeSelectedCommand
        public ICommand EmployeeSelectedCommand
        {
            get { return (ICommand)GetValue(EmployeeSelectedCommandProperty); }
            set { SetValue(EmployeeSelectedCommandProperty, value); }
        }

        public static readonly DependencyProperty EmployeeSelectedCommandProperty = DependencyProperty.Register(nameof(EmployeeSelectedCommand), typeof(ICommand), typeof(EmployeeSuggestBox), new PropertyMetadata(null));
        #endregion

        private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (args.CheckCurrent())
                {
                    Items = String.IsNullOrEmpty(sender.Text) ? null : await GetItems(sender.Text);
                }
            }
        }

        private async Task<IList<EmployeeModel>> GetItems(string query)
        {
            var request = new DataRequest<Data.Data.Employee>()
            {
                Query = query,
                OrderBy = r => r.FirstName
            };
            return await EmployeeService.GetEmployeesAsync(0, 20, request);
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            EmployeeSelectedCommand?.TryExecute(args.SelectedItem);
        }
    }
}
