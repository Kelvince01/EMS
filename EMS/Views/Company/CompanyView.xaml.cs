using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.ViewModels.Company;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EMS.Views.Company
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompanyView : Page
    {
        public CompanyView()
        {
            ViewModel = ServiceLocator.Current.GetService<CompanyDetailsViewModel>();
            NavigationService = ServiceLocator.Current.GetService<INavigationService>();
            this.InitializeComponent();
        }

        public CompanyDetailsViewModel ViewModel { get; }
        public INavigationService NavigationService { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as CompanyDetailsArgs);

            if (ViewModel.IsEditMode)
            {
                await Task.Delay(100);
                details.SetFocus();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        /*private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            ViewModel.CompanyDetails.CancelEdit();
            if (pivot.SelectedIndex == 0)
            {
                await NavigationService.CreateNewViewAsync<CompanyDetailsViewModel>(ViewModel.CompanyDetails.CreateArgs());
            }
        }*/

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }
    }
}
