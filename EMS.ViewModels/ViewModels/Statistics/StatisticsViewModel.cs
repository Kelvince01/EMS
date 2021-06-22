using System.Threading.Tasks;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;

namespace EMS.ViewModels.ViewModels.Statistics
{
    #region StatisticsArgs
    public class StatisticsArgs
    {
        static public StatisticsArgs CreateDefault() => new StatisticsArgs();
    }
    #endregion

    public class StatisticsViewModel : ViewModelBase
    {
        public StatisticsViewModel(ICommonServices commonServices) : base(commonServices)
        {

        }

        public StatisticsArgs ViewModelArgs { get; private set; }

        public Task LoadAsync(StatisticsArgs args)
        {
            ViewModelArgs = args ?? StatisticsArgs.CreateDefault();

            StatusReady();

            return Task.CompletedTask;
        }
    }
}