using EMS.ViewModels.ViewModels.Company;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EMS.Views.Companys.Details
{
    public sealed partial class CompanysDetails : UserControl
    {
        public CompanysDetails()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public CompanyDetailsViewModel ViewModel
        {
            get { return (CompanyDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(CompanyDetailsViewModel), typeof(CompanysDetails), new PropertyMetadata(null));
        #endregion
    }
}
