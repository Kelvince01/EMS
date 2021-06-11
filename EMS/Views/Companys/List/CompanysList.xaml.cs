using EMS.ViewModels.ViewModels.Company;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EMS.Views.Companys.List
{
    public sealed partial class CompanysList : UserControl
    {
        public CompanysList()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public CompanyListViewModel ViewModel
        {
            get { return (CompanyListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CompanyListViewModel), typeof(CompanysList), new PropertyMetadata(null));
        #endregion
    }
}
