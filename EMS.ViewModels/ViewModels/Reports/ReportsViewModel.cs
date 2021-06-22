using System.Threading.Tasks;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;

namespace EMS.ViewModels.ViewModels.Reports
{
    #region ReportsArgs
    public class ReportsArgs
    {
        static public ReportsArgs CreateDefault() => new ReportsArgs();
    }
    #endregion

    public class ReportsViewModel : ViewModelBase
    {
        public ReportsViewModel(ICommonServices commonServices) : base(commonServices)
        {

        }

        public ReportsArgs ViewModelArgs { get; private set; }

        public Task LoadAsync(ReportsArgs args)
        {
            ViewModelArgs = args ?? ReportsArgs.CreateDefault();

            StatusReady();

            return Task.CompletedTask;
        }
    }
}