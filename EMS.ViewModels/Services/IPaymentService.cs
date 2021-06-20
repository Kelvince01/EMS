using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.ViewModels.Models;

namespace EMS.ViewModels.Services
{
    public interface IPaymentService
    {
        Task<PaymentModel> GetPaymentAsync(long id);
        Task<IList<PaymentModel>> GetPaymentsAsync(DataRequest<Payment> request);
        Task<IList<PaymentModel>> GetPaymentsAsync(int skip, int take, DataRequest<Payment> request);
        Task<int> GetPaymentsCountAsync(DataRequest<Payment> request);

        Task<int> UpdatePaymentAsync(PaymentModel model);

        //Task<int> DeletePaymentAsync(PaymentModel model);
        //Task<int> DeletePaymentRangeAsync(int index, int length, DataRequest<Payment> request);
    }
}