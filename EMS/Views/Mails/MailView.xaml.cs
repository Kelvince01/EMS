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
using EMS.Extensions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EMS.Views.Mails
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MailView : Page
    {
        public MailView()
        {
            this.InitializeComponent();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEmailMessagesFromOffice365();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadEmailMessagesFromOffice365();
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }


        private async void LoadEmailMessagesFromOffice365()
        {
            MasterListView.ItemsSource = null;
            progressRing.IsActive = true;

            var outlookClient = await AuthUtil.EnsureClient();

            var messages = await
                outlookClient.Me.Folders["Inbox"].Messages.
                    OrderByDescending(m => m.DateTimeReceived).Take(50).
                    ExecuteAsync();

            progressRing.IsActive = true;

            MasterListView.ItemsSource = messages.CurrentPage;
        }
    }
}
