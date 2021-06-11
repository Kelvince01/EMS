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
    public class ProjectCollection : VirtualCollection<ProjectModel>
    {
        private DataRequest<Project> _dataRequest = null;

        public ProjectCollection(IProjectService projectService, ILogService logService) : base(logService)
        {
            ProjectService = projectService;
        }

        public IProjectService ProjectService { get; }

        private ProjectModel _defaultItem = ProjectModel.CreateEmpty();
        protected override ProjectModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<Project> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await ProjectService.GetProjectsCountAsync(_dataRequest);
                Ranges[0] = await ProjectService.GetProjectsAsync(0, RangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<ProjectModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await ProjectService.GetProjectsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                LogException("ProjectCollection", "Fetch", ex);
            }
            return null;
        }
    }
}
