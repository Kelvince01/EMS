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
using EMS.ViewModels.ViewModels.Company;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EMS.Views.Company.Details
{
    public sealed partial class CompanyDetails : UserControl
    {
        public CompanyDetails()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public CompanyDetailsViewModel ViewModel
        {
            get { return (CompanyDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CompanyDetailsViewModel), typeof(CompanyDetails), new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            details.SetFocus();
        }
    }
}
