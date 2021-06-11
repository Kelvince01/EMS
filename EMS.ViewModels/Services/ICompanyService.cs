using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.ViewModels.Models;

namespace EMS.ViewModels.Services
{
    public interface ICompanyService
    {
        Task<CompanyModel> GetCompanyAsync(long id);
        Task<int> UpdateCompanyAsync(CompanyModel model);
        Task<int> DeleteCompanyAsync(CompanyModel model);
        Task<int> GetCompanysCountAsync(DataRequest<Company> request);
        Task<IList<CompanyModel>> GetCompanysAsync(DataRequest<Company> request);
        Task<IList<CompanyModel>> GetCompanysAsync(int skip, int take, DataRequest<Company> request);
        Task<int> DeleteCompanyRangeAsync(int index, int length, DataRequest<Company> request);
    }
}