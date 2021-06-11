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
using Microsoft.EntityFrameworkCore;

namespace EMS.Data.DataServices.Base
{
    partial class DataServiceBase
    {
        public async Task<Project> GetProjectAsync(long id)
        {
            return await _dataSource.Projects.Where(r => r.ProjectID == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Project>> GetProjectsAsync(int skip, int take, DataRequest<Project> request)
        {
            IQueryable<Project> items = GetProjects(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Project>> GetProjectKeysAsync(int skip, int take, DataRequest<Project> request)
        {
            IQueryable<Project> items = GetProjects(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Project
                {
                    ProjectID = r.ProjectID,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Project> GetProjects(DataRequest<Project> request)
        {
            IQueryable<Project> items = _dataSource.Projects;

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

        public async Task<int> GetProjectsCountAsync(DataRequest<Project> request)
        {
            IQueryable<Project> items = _dataSource.Projects;

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

        public async Task<int> UpdateProjectAsync(Project Project)
        {
            if (Project.ProjectID > 0)
            {
                _dataSource.Entry(Project).State = EntityState.Modified;
            }
            else
            {
                Project.ProjectID = UIDGenerator.Next();
                Project.CreatedOn = DateTime.UtcNow;
                _dataSource.Entry(Project).State = EntityState.Added;
            }
            Project.LastModifiedOn = DateTime.UtcNow;
            Project.SearchTerms = Project.BuildSearchTerms();
            return await _dataSource.SaveChangesAsync();
        }

        public async Task<int> DeleteProjectsAsync(params Project[] Projects)
        {
            _dataSource.Projects.RemoveRange(Projects);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
