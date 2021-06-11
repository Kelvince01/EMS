#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
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
    public class ProjectService : IProjectService
    {
        public ProjectService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<ProjectModel> GetProjectAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetProjectAsync(dataService, id);
            }
        }
        static private async Task<ProjectModel> GetProjectAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetProjectAsync(id);
            if (item != null)
            {
                return await CreateProjectModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<ProjectModel>> GetProjectsAsync(DataRequest<Project> request)
        {
            var collection = new ProjectCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<ProjectModel>> GetProjectsAsync(int skip, int take, DataRequest<Project> request)
        {
            var models = new List<ProjectModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetProjectsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateProjectModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetProjectsCountAsync(DataRequest<Project> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetProjectsCountAsync(request);
            }
        }

        public async Task<int> UpdateProjectAsync(ProjectModel model)
        {
            long id = model.ProjectID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var project = id > 0 ? await dataService.GetProjectAsync(model.ProjectID) : new Project();
                if (project != null)
                {
                    UpdateProjectFromModel(project, model);
                    await dataService.UpdateProjectAsync(project);
                    model.Merge(await GetProjectAsync(dataService, project.ProjectID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteProjectAsync(ProjectModel model)
        {
            var project = new Project { ProjectID = model.ProjectID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteProjectsAsync(project);
            }
        }

        public async Task<int> DeleteProjectRangeAsync(int index, int length, DataRequest<Project> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetProjectKeysAsync(index, length, request);
                return await dataService.DeleteProjectsAsync(items.ToArray());
            }
        }

        static public async Task<ProjectModel> CreateProjectModelAsync(Project source, bool includeAllFields)
        {
            var model = new ProjectModel()
            {
                ProjectID = source.ProjectID,
                CategoryID = source.CategoryID,
                Name = source.Name,
                Description = source.Description,
                Size = source.Size,
                Color = source.Color,
                ListPrice = source.ListPrice,
                DealerPrice = source.DealerPrice,
                TaxType = source.TaxType,
                Discount = source.Discount,
                DiscountStartDate = source.DiscountStartDate,
                DiscountEndDate = source.DiscountEndDate,
                StockUnits = source.StockUnits,
                SafetyStockLevel = source.SafetyStockLevel,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail,
                ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail)
            };

            if (includeAllFields)
            {
                model.Picture = source.Picture;
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
            }
            return model;
        }

        private void UpdateProjectFromModel(Project target, ProjectModel source)
        {
            target.CategoryID = source.CategoryID;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Size = source.Size;
            target.Color = source.Color;
            target.ListPrice = source.ListPrice;
            target.DealerPrice = source.DealerPrice;
            target.TaxType = source.TaxType;
            target.Discount = source.Discount;
            target.DiscountStartDate = source.DiscountStartDate;
            target.DiscountEndDate = source.DiscountEndDate;
            target.StockUnits = source.StockUnits;
            target.SafetyStockLevel = source.SafetyStockLevel;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }
    }
}
