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

        public async Task<ProjectModel> CreateNewProjectAsync(long employeeID)
        {
            var model = new ProjectModel
            {
                EmployeeID = employeeID,
                CreatedOn = DateTime.UtcNow,
                Status = 0
            };
            if (employeeID > 0)
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var parent = await dataService.GetEmployeeAsync(employeeID);
                    if (parent != null)
                    {
                        model.EmployeeID = employeeID;
                        model.Employee = await EmployeeService.CreateEmployeeModelAsync(parent, includeAllFields: true);
                    }
                }
            }
            return model;
        }

        static public async Task<ProjectModel> CreateProjectModelAsync(Project source, bool includeAllFields)
        {
            var model = new ProjectModel()
            {
                ProjectID = source.ProjectID,
                CategoryID = source.CategoryID,
                EmployeeID = source.EmployeeID,
                CustomerID = source.CustomerID,
                Name = source.Name,
                Description = source.Description,
                Price = source.Price,
                TaxType = source.TaxType,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Status = source.Status,
                Progress = source.Progress,
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
            target.EmployeeID = source.EmployeeID;
            target.CustomerID = source.CustomerID;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Price = source.Price;
            target.TaxType = source.TaxType;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Status = source.Status;
            target.Progress = source.Progress;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }
    }
}
