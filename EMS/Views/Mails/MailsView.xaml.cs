using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using EMS.Extensions;
using EMS.Tools;
using EMS.Views.Mails.Parts;

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
            this.Loaded += new RoutedEventHandler(AppRoot_Loaded);
        }

        void AppRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (!HelperUtils.AreSettingsAvailable())
            {
                ContentFrame.Navigate(typeof(MailSettingsPage));
            }
            else
                ContentFrame.Navigate(typeof(MailMainPage));
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {

            // set the initial SelectedItem 
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "mail")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(MailSettingsPage));
            }
            else
            {
                switch (args.InvokedItem)
                {
                    case "Home":
                        MailHandler.ReplyFlag = false;
                        MailHandler.ForwardFlag = false;
                        ContentFrame.Navigate(typeof(MailMainPage));
                        break;

                    case "Send Mail":
                        MailHandler.ReplyFlag = false;
                        MailHandler.ForwardFlag = false;
                        ContentFrame.Navigate(typeof(CreateMailPage));
                        break;
                }
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(MailSettingsPage));
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                switch (item.Tag)
                {
                    case "home":
                        MailHandler.ReplyFlag = false;
                        MailHandler.ForwardFlag = false;
                        ContentFrame.Navigate(typeof(MailMainPage));
                        break;

                    case "mail":
                        MailHandler.ReplyFlag = false;
                        MailHandler.ForwardFlag = false;
                        ContentFrame.Navigate(typeof(CreateMailPage));
                        break;
                }
            }
        }
    }
}
