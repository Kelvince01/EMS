using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using EMS.Extensions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EMS.Views.Mails
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MailsView : Page
    {
        public MailsView()
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
