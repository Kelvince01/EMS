using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EMS.ViewModels.Models;
using EMS.ViewModels.ViewModels.Company;
using EMS.ViewModels.ViewModels.Employees;
using EMS.Views.Employee.Details;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EMS.Views.Company.Details
{
    public sealed partial class CompanyCard : UserControl
    {
        public CompanyCard()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public CompanyDetailsViewModel ViewModel
        {
            get { return (CompanyDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(CompanyDetailsViewModel), typeof(CompanyCard), new PropertyMetadata(null));
        #endregion

        #region Item
        public CompanyModel Item
        {
            get { return (CompanyModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(CompanyModel), typeof(CompanyCard), new PropertyMetadata(null));
        #endregion
    }
}
