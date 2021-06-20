using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EMS.Views.Charts
{
    public sealed partial class WebviewView : UserControl
    {
        public WebviewView()
        {
            this.InitializeComponent();
            webView.ScriptNotify += webView_ScriptNotify;
        }

        async void webView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var jsScriptValue = e.Value;
            MessageDialog msg = new MessageDialog(jsScriptValue);
            var res = await msg.ShowAsync();

        }

        private const string htmlFragment =
            "<html><head><script type='text/javascript'>" +
            "function doubleIt(incoming){ " +
            "  var intIncoming = parseInt(incoming, 10);" +
            "  var doubled = intIncoming * 2;" +
            "  document.body.style.fontSize= doubled.toString() + 'px';" +
            "  window.external.notify('The script says the doubled value is ' + doubled.toString());" +
            "};" +
            "</script></head><body><div id='myDiv'>I AM CONTENT</div></body></html>";

        /*protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            webView.NavigateToString(htmlFragment);
        }*/

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            webView.NavigateToString(htmlFragment);
        }

        private void Change25_Click(object sender, RoutedEventArgs e)
        {
            webView.InvokeScript("doubleIt", new string[] { "25" });
        }

        private void Change40_Click(object sender, RoutedEventArgs e)
        {
            webView.InvokeScript("doubleIt", new string[] { "40" });
        }

        private void Change60_Click(object sender, RoutedEventArgs e)
        {
            webView.InvokeScript("doubleIt", new string[] { "60" });
        }
    }
}
