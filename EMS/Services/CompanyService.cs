using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.Data.DataServices;
using EMS.Services.DataServiceFactory;
using EMS.Services.VirtualCollections;
using EMS.Tools;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.Services
{
    public class CompanyService : ICompanyService
    {
        public CompanyService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<CompanyModel> GetCompanyAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetCompanyAsync(dataService, id);
            }
        }

        static private async Task<CompanyModel> GetCompanyAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetCompanyAsync(id);
            if (item != null)
            {
                return await CreateCompanyModelAsync(item);
            }
            return null;
        }

        public async Task<int> UpdateCompanyAsync(CompanyModel model)
        {
            long id = model.CompanyID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var company = id > 0 ? await dataService.GetCompanyAsync(model.CompanyID) : new Company();
                if (company != null)
                {
                    UpdateCompanyFromModel(company, model);
                    await dataService.UpdateCompanyAsync(company);
                    model.Merge(await GetCompanyAsync(dataService, company.CompanyID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteCompanyAsync(CompanyModel model)
        {
            var company = new Company { CompanyID = model.CompanyID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteCompanysAsync(company);
            }
        }

        public async Task<IList<CompanyModel>> GetCompanysAsync(DataRequest<Company> request)
        {
            var collection = new CompanyCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<CompanyModel>> GetCompanysAsync(int skip, int take, DataRequest<Company> request)
        {
            var models = new List<CompanyModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetCompanysAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateCompanyModelAsync(item));
                }
                return models;
            }
        }

        public async Task<int> DeleteCompanyRangeAsync(int index, int length, DataRequest<Company> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetCompanyKeysAsync(index, length, request);
                return await dataService.DeleteCompanysAsync(items.ToArray());
            }
        }

        public async Task<int> GetCompanysCountAsync(DataRequest<Company> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetCompanysCountAsync(request);
            }
        }

        static public async Task<CompanyModel> CreateCompanyModelAsync(Company source)
        {
            var model = new CompanyModel()
            {
                CompanyID = source.CompanyID,
                Name = source.Name,
                Description = source.Description,
                Address = source.Address,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                PhoneNumber = source.PhoneNumber,
                Email = source.Email,
                Website = source.Website,
                Contact = source.Contact,
                ABN = source.ABN,
                ACN = source.ACN,
                Picture = source.Picture,
                PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture)
        };
            
            return model;
        }

        private void UpdateCompanyFromModel(Company target, CompanyModel source)
        {
            target.Name = source.Name;
            target.Description = source.Description;
            target.Address = source.Address;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.PhoneNumber = source.PhoneNumber;
            target.Email = source.Email;
            target.Website = source.Website;
            target.Contact = source.Contact;
            target.ABN = source.ABN;
            target.ACN = source.ACN;
            target.Picture = source.Picture;
        }
    }
}