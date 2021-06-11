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
using System.Threading.Tasks;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.ViewModels.ViewModels.Projects
{
    public class ProjectsViewModel : ViewModelBase
    {
        public ProjectsViewModel(IProjectService projectService, IOrderService orderService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            ProjectService = projectService;

            ProjectList = new ProjectListViewModel(ProjectService, commonServices);
            ProjectDetails = new ProjectDetailsViewModel(ProjectService, filePickerService, commonServices);
        }

        public IProjectService ProjectService { get; }

        public ProjectListViewModel ProjectList { get; set; }
        public ProjectDetailsViewModel ProjectDetails { get; set; }

        public async Task LoadAsync(ProjectListArgs args)
        {
            await ProjectList.LoadAsync(args);
        }
        public void Unload()
        {
            ProjectDetails.CancelEdit();
            ProjectList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<ProjectListViewModel>(this, OnMessage);
            ProjectList.Subscribe();
            ProjectDetails.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            ProjectList.Unsubscribe();
            ProjectDetails.Unsubscribe();
        }

        private async void OnMessage(ProjectListViewModel viewModel, string message, object args)
        {
            if (viewModel == ProjectList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (ProjectDetails.IsEditMode)
            {
                StatusReady();
                ProjectDetails.CancelEdit();
            }
            var selected = ProjectList.SelectedItem;
            if (!ProjectList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            ProjectDetails.Item = selected;
        }

        private async Task PopulateDetails(ProjectModel selected)
        {
            try
            {
                var model = await ProjectService.GetProjectAsync(selected.ProjectID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Projects", "Load Details", ex);
            }
        }
    }
}
