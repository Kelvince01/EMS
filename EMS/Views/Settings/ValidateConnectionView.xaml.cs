using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EMS.Configuration;
using EMS.ViewModels.Infrastructure.Common;
using EMS.ViewModels.ViewModels.Settings;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EMS.Views.Settings
{
    public sealed partial class ValidateConnectionView : ContentDialog
    {
        private string _connectionString = null;

        public ValidateConnectionView(string connectionString)
        {
            _connectionString = connectionString;
            ViewModel = ServiceLocator.Current.GetService<ValidateConnectionViewModel>();
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ValidateConnectionViewModel ViewModel { get; }

        public Result Result { get; private set; }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.ExecuteAsync(_connectionString);
            Result = ViewModel.Result;
        }

        private void OnOkClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnCancelClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = Result.Ok("Operation cancelled by user");
        }
    }
}
