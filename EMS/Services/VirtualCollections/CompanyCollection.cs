using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Common.VirtualCollection;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.Services.VirtualCollections
{
    public class CompanyCollection : VirtualCollection<CompanyModel>
    {
        private DataRequest<Company> _dataRequest = null;

        public CompanyCollection(ICompanyService companyService, ILogService logService) : base(logService)
        {
            CompanyService = companyService;
        }

        public ICompanyService CompanyService { get; }

        private CompanyModel _defaultItem = CompanyModel.CreateEmpty();
        protected override CompanyModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<Company> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await CompanyService.GetCompanysCountAsync(_dataRequest);
                Ranges[0] = await CompanyService.GetCompanysAsync(0, RangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<CompanyModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await CompanyService.GetCompanysAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                LogException("CompanyCollection", "Fetch", ex);
            }
            return null;
        }
    }
}