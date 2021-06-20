using System.Threading.Tasks;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;

namespace EMS.ViewModels.ViewModels.Charts
{
    #region ChartsArgs
    public class ChartsArgs
    {
        static public ChartsArgs CreateDefault() => new ChartsArgs();
    }
    #endregion

    public class ChartsViewModel : ViewModelBase
    {
        public ChartsViewModel(ICommonServices commonServices) : base(commonServices)
        {
            
        }

        public ChartsArgs ViewModelArgs { get; private set; }

        public Task LoadAsync(ChartsArgs args)
        {
            ViewModelArgs = args ?? ChartsArgs.CreateDefault();

            StatusReady();

            return Task.CompletedTask;
        }
    }
}