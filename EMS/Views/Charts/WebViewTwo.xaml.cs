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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EMS.Views.Charts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebViewTwo : Page
    {
        public WebViewTwo()
        {
            this.InitializeComponent();
            MyWebView.UnviewableContentIdentified += MyWebView_UnviewableContentIdentified;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var uristring = e.Parameter as string;
            if (uristring != "")
            {
                string[] parts = uristring.ToString().Split(':');
                string p = parts[1].Substring(1, parts[1].Length - 1);
                uristring = "https://somaliastabilityke.sharepoint.com/sites/ssf2" + p;
                Uri u = new Uri(uristring);
                MyWebView.Navigate(u);
            }
            else
            {
                Uri u = new Uri("https://somaliastabilityke.sharepoint.com/sites/ssf2");
                MyWebView.Navigate(u);
            }
        }

        private void MyWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ProgressRing1.IsActive = true;
        }

        private void MyWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            ProgressRing1.IsActive = false; //for progress ring  
        }

        private void MyWebView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            Windows.Foundation.IAsyncOperation<bool> b =
                Windows.System.Launcher.LaunchUriAsync(args.Uri);
        }

    }
}
