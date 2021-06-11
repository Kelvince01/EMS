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
    public sealed partial class ProjectSuggestBox : UserControl
    {
        public ProjectSuggestBox()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                ProjectService = ServiceLocator.Current.GetService<IProjectService>();
            }
            InitializeComponent();
        }

        private IProjectService ProjectService { get; }

        #region Items
        public IList<ProjectModel> Items
        {
            get { return (IList<ProjectModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<ProjectModel>), typeof(ProjectSuggestBox), new PropertyMetadata(null));
        #endregion

        #region DisplayText
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(ProjectSuggestBox), new PropertyMetadata(null));
        #endregion

        #region IsReadOnly*
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProjectSuggestBox;
            control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ProjectSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
        #endregion

        #region ProjectSelectedCommand
        public ICommand ProjectSelectedCommand
        {
            get { return (ICommand)GetValue(ProjectSelectedCommandProperty); }
            set { SetValue(ProjectSelectedCommandProperty, value); }
        }

        public static readonly DependencyProperty ProjectSelectedCommandProperty = DependencyProperty.Register(nameof(ProjectSelectedCommand), typeof(ICommand), typeof(ProjectSuggestBox), new PropertyMetadata(null));
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

        private async Task<IList<ProjectModel>> GetItems(string query)
        {
            var request = new DataRequest<Data.Data.Project>()
            {
                Query = query,
                OrderBy = r => r.Name
            };
            return await ProjectService.GetProjectsAsync(0, 20, request);
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ProjectSelectedCommand?.TryExecute(args.SelectedItem);
        }
    }
}
