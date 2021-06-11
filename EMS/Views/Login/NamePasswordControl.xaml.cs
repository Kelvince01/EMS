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
    public sealed partial class NamePasswordControl : UserControl
    {
        public NamePasswordControl()
        {
            InitializeComponent();
        }

        #region UserName
        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(NamePasswordControl), new PropertyMetadata(null));
        #endregion

        #region Password
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(string), typeof(NamePasswordControl), new PropertyMetadata(null));
        #endregion

        #region LoginWithPasswordCommand
        public ICommand LoginWithPasswordCommand
        {
            get { return (ICommand)GetValue(LoginWithPasswordCommandProperty); }
            set { SetValue(LoginWithPasswordCommandProperty, value); }
        }

        public static readonly DependencyProperty LoginWithPasswordCommandProperty = DependencyProperty.Register(nameof(LoginWithPasswordCommand), typeof(ICommand), typeof(NamePasswordControl), new PropertyMetadata(null));
        #endregion

        public void Focus()
        {
            userName.Focus(FocusState.Programmatic);
        }
    }
}
