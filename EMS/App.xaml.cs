using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EMS.Configuration;
using EMS.Services.Infrastructure;
using EMS.ViewModels.Infrastructure.Services;
using EMS.Views.SplashScreen;

namespace EMS
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(1280, 840);

            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await ActivateAsync(e);
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            await ActivateAsync(e);
        }

        private async Task ActivateAsync(IActivatedEventArgs e)
        {
            var activationInfo = ActivationService.GetActivationInfo(e);

            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                ExtendedSplash extendedSplash = new ExtendedSplash(e, loadState);
                Window.Current.Content = extendedSplash;
                Window.Current.Activate();
            }
            else
            {
                var navigationService = ServiceLocator.Current.GetService<INavigationService>();
                await navigationService.CreateNewViewAsync(activationInfo.EntryViewModel, activationInfo.EntryArgs);
            }
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var logService = ServiceLocator.Current.GetService<ILogService>();
            await logService.WriteAsync(Data.Data.LogType.Information, "App", "Suspending", "Application End", $"Application ended by '{AppSettings.Current.UserName}'.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var logService = ServiceLocator.Current.GetService<ILogService>();
            logService.WriteAsync(Data.Data.LogType.Error, "App", "UnhandledException", e.Message, e.Exception.ToString());
        }
    }
}
