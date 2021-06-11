using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using EMS.Configuration;
using EMS.ViewModels.ViewModels.Dashboard;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EMS.Views.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardView : Page
    {
        public DashboardView()
        {
            ViewModel = ServiceLocator.Current.GetService<DashboardViewModel>();
            InitializeComponent();
        }

        public DashboardViewModel ViewModel { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.LoadAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Unload();
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Control control)
            {
                ViewModel.ItemSelected(control.Tag as String);
            }
        }
    }
}
