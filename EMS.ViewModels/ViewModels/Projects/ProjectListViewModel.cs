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
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using EMS.Data.Common;
using EMS.Data.Data;
using EMS.ViewModels.Infrastructure.Common;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.Common;

namespace EMS.ViewModels.ViewModels.Projects
{
    #region ProjectListArgs
    public class ProjectListArgs
    {
        static public ProjectListArgs CreateEmpty() => new ProjectListArgs { IsEmpty = true };

        public ProjectListArgs()
        {
            OrderBy = r => r.Name;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<Project, object>> OrderBy { get; set; }
        public Expression<Func<Project, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class ProjectListViewModel : GenericListViewModel<ProjectModel>
    {
        public ProjectListViewModel(IProjectService projectService, ICommonServices commonServices) : base(commonServices)
        {
            ProjectService = projectService;
        }

        public IProjectService ProjectService { get; }

        public ProjectListArgs ViewModelArgs { get; private set; }

        public ICommand ItemInvokedCommand => new RelayCommand<ProjectModel>(ItemInvoked);
        private async void ItemInvoked(ProjectModel model)
        {
            await NavigationService.CreateNewViewAsync<ProjectDetailsViewModel>(new ProjectDetailsArgs { ProjectID = model.ProjectID });
        }

        public async Task LoadAsync(ProjectListArgs args)
        {
            ViewModelArgs = args ?? ProjectListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading Projects...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Projects loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<ProjectListViewModel>(this, OnMessage);
            MessageService.Subscribe<ProjectDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public ProjectListArgs CreateArgs()
        {
            return new ProjectListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;

            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            try
            {
                Items = await GetItemsAsync();
            }
            catch (Exception ex)
            {
                Items = new List<ProjectModel>();
                StatusError($"Error loading Projects: {ex.Message}");
                LogException("Projects", "Refresh", ex);
                isOk = false;
            }

            ItemsCount = Items.Count;
            if (!IsMultipleSelection)
            {
                SelectedItem = Items.FirstOrDefault();
            }
            NotifyPropertyChanged(nameof(Title));

            return isOk;
        }

        private async Task<IList<ProjectModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Project> request = BuildDataRequest();
                return await ProjectService.GetProjectsAsync(request);
            }
            return new List<ProjectModel>();
        }

        protected override async void OnNew()
        {

            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<ProjectDetailsViewModel>(new ProjectDetailsArgs());
            }
            else
            {
                NavigationService.Navigate<ProjectDetailsViewModel>(new ProjectDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading Projects...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Projects loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected Projects?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} Projects...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} Projects...");
                        await DeleteItemsAsync(SelectedItems);
                        MessageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Projects: {ex.Message}");
                    LogException("Projects", "Delete", ex);
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} Projects deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<ProjectModel> models)
        {
            foreach (var model in models)
            {
                await ProjectService.DeleteProjectAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Project> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await ProjectService.DeleteProjectRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Project> BuildDataRequest()
        {
            return new DataRequest<Project>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        private async void OnMessage(ViewModelBase sender, string message, object args)
        {
            switch (message)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await ContextService.RunAsync(async () =>
                    {
                        await RefreshAsync();
                    });
                    break;
            }
        }
    }
}
