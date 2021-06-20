using System.Threading.Tasks;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.ViewModels.Common;

namespace EMS.ViewModels.ViewModels.Payments
{
    public class PaymentDetailsViewModel : GenericDetailsViewModel<PaymentModel>
    {
        public PaymentDetailsViewModel(ICommonServices commonServices) : base(commonServices)
        {
        }

        public override bool ItemIsNew { get; }
        protected override Task<bool> SaveItemAsync(PaymentModel model)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<bool> DeleteItemAsync(PaymentModel model)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<bool> ConfirmDeleteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}