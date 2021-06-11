using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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

namespace EMS.Views.Login
{
    public sealed partial class WindowsHelloControl : UserControl
    {
        public WindowsHelloControl()
        {
            InitializeComponent();
        }

        #region UserName
        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(WindowsHelloControl), new PropertyMetadata(null));
        #endregion

        #region LoginWithWindowHelloCommand
        public ICommand LoginWithWindowHelloCommand
        {
            get { return (ICommand)GetValue(LoginWithWindowHelloCommandProperty); }
            set { SetValue(LoginWithWindowHelloCommandProperty, value); }
        }

        public static readonly DependencyProperty LoginWithWindowHelloCommandProperty = DependencyProperty.Register(nameof(LoginWithWindowHelloCommand), typeof(ICommand), typeof(WindowsHelloControl), new PropertyMetadata(null));
        #endregion
    }
}
