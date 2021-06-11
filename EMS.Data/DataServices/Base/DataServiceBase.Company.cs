using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data.Common;
using EMS.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.Data.DataServices.Base
{
    partial class DataServiceBase
    {
        public async Task<Company> GetCompanyAsync(long id)
        {
            return await _dataSource.Companys.Where(r => r.CompanyID == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Company>> GetCompanysAsync(int skip, int take, DataRequest<Company> request)
        {
            IQueryable<Company> items = GetCompanys(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Company
                {
                    CompanyID = r.CompanyID,
                    Name = r.Name,
                    Description = r.Description,
                    Address = r.Address,
                    CreatedOn = r.CreatedOn,
                    LastModifiedOn = r.LastModifiedOn,
                    PhoneNumber = r.PhoneNumber,
                    Email = r.Email,
                    Website = r.Website,
                    Contact = r.Contact,
                    ABN = r.ABN,
                    ACN = r.ACN,
                    Picture = r.Picture,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Company>> GetCompanyKeysAsync(int skip, int take, DataRequest<Company> request)
        {
            IQueryable<Company> items = GetCompanys(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Company
                {
                    CompanyID = r.CompanyID,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Company> GetCompanys(DataRequest<Company> request)
        {
            IQueryable<Company> items = _dataSource.Companys;

            // Query
            if (!String.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order By
            if (request.OrderBy != null)
            {
                items = items.OrderBy(request.OrderBy);
            }
            if (request.OrderByDesc != null)
            {
                items = items.OrderByDescending(request.OrderByDesc);
            }

            return items;
        }

        public async Task<int> GetCompanysCountAsync(DataRequest<Company> request)
        {
            IQueryable<Company> items = _dataSource.Companys;

            // Query
            if (!String.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateCompanyAsync(Company company)
        {
            if (company.CompanyID > 0)
            {
                _dataSource.Entry(company).State = EntityState.Modified;
            }
            else
            {
                company.CompanyID = UIDGenerator.Next();
                company.CreatedOn = DateTime.UtcNow;
                _dataSource.Entry(company).State = EntityState.Added;
            }
            company.LastModifiedOn = DateTime.UtcNow;
            company.SearchTerms = company.BuildSearchTerms();
            int res = await _dataSource.SaveChangesAsync();
            return res;
        }

        public async Task<int> DeleteCompanysAsync(params Company[] companys)
        {
            _dataSource.Companys.RemoveRange(companys);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
