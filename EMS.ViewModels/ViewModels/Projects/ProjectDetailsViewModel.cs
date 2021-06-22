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
using System.Windows.Input;
using EMS.ViewModels.Infrastructure.Common;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.Common;

namespace EMS.ViewModels.ViewModels.Projects
{
    #region ProjectDetailsArgs
    public class ProjectDetailsArgs
    {
        static public ProjectDetailsArgs CreateDefault() => new ProjectDetailsArgs { CustomerID = 0, EmployeeID = 0};

        public long ProjectID { get; set; }

        public long CustomerID { get; set; }
        
        public long EmployeeID { get; set; }

        public bool IsNew => ProjectID <= 0;
    }
    #endregion

    public class ProjectDetailsViewModel : GenericDetailsViewModel<ProjectModel>
    {
        public ProjectDetailsViewModel(IProjectService projectService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            ProjectService = projectService;
            FilePickerService = filePickerService;
        }

        public IProjectService ProjectService { get; }
        public IFilePickerService FilePickerService { get; }

        override public string Title => (Item?.IsNew ?? true) ? "New Project" : TitleEdit;
        public string TitleEdit => Item == null ? "Project" : $"{Item.Name}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditCustomer => Item?.CustomerID <= 0;

        public ICommand CustomerSelectedCommand => new RelayCommand<CustomerModel>(CustomerSelected);
        private void CustomerSelected(CustomerModel customer)
        {
            EditableItem.CustomerID = customer.CustomerID;
            EditableItem.Customer = customer;

            EditableItem.NotifyChanges();
        }

        public bool CanEditEmployee => Item?.EmployeeID <= 0;

        public ICommand EmployeeSelectedCommand => new RelayCommand<EmployeeModel>(EmployeeSelected);
        private void EmployeeSelected(EmployeeModel employee)
        {
            EditableItem.EmployeeID = employee.EmployeeID;
            EditableItem.Employee = employee;

            EditableItem.NotifyChanges();
        }

        public ProjectDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(ProjectDetailsArgs args)
        {
            ViewModelArgs = args ?? ProjectDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                //Item = new ProjectModel();
                //IsEditMode = true;

                Item = await ProjectService.CreateNewProjectAsync(ViewModelArgs.EmployeeID);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await ProjectService.GetProjectAsync(ViewModelArgs.ProjectID);
                    Item = item ?? new ProjectModel { ProjectID = ViewModelArgs.ProjectID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Project", "Load", ex);
                }
            }
            NotifyPropertyChanged(nameof(ItemIsNew));
        }
        public void Unload()
        {
            ViewModelArgs.ProjectID = Item?.ProjectID ?? 0;
            ViewModelArgs.EmployeeID = Item?.EmployeeID ?? 0;
            ViewModelArgs.CustomerID = Item?.CustomerID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<ProjectDetailsViewModel, ProjectModel>(this, OnDetailsMessage);
            MessageService.Subscribe<ProjectListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public ProjectDetailsArgs CreateArgs()
        {
            return new ProjectDetailsArgs
            {
                CustomerID = Item?.CustomerID ?? 0,
                EmployeeID = Item?.EmployeeID ?? 0,
                ProjectID = Item?.ProjectID ?? 0
            };
        }

        private object _newPictureSource = null;
        public object NewPictureSource
        {
            get => _newPictureSource;
            set => Set(ref _newPictureSource, value);
        }

        public override void BeginEdit()
        {
            NewPictureSource = null;
            base.BeginEdit();
        }

        public ICommand EditPictureCommand => new RelayCommand(OnEditPicture);
        private async void OnEditPicture()
        {
            NewPictureSource = null;
            var result = await FilePickerService.OpenImagePickerAsync();
            if (result != null)
            {
                EditableItem.Picture = result.ImageBytes;
                EditableItem.PictureSource = result.ImageSource;
                EditableItem.Thumbnail = result.ImageBytes;
                EditableItem.ThumbnailSource = result.ImageSource;
                NewPictureSource = result.ImageSource;
            }
            else
            {
                NewPictureSource = null;
            }
        }

        protected override async Task<bool> SaveItemAsync(ProjectModel model)
        {
            try
            {
                StartStatusMessage("Saving Project...");
                await Task.Delay(100);
                await ProjectService.UpdateProjectAsync(model);
                EndStatusMessage("Project saved");
                LogInformation("Project", "Save", "Project saved successfully", $"Project {model.ProjectID} '{model.Name}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Project: {ex.Message}");
                LogException("Project", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(ProjectModel model)
        {
            try
            {
                StartStatusMessage("Deleting Project...");
                await Task.Delay(100);
                await ProjectService.DeleteProjectAsync(model);
                EndStatusMessage("Project deleted");
                LogWarning("Project", "Delete", "Project deleted", $"Project {model.ProjectID} '{model.Name}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Project: {ex.Message}");
                LogException("Project", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current Project?", "Ok", "Cancel");
        }

        override protected IEnumerable<IValidationConstraint<ProjectModel>> GetValidationConstraints(ProjectModel model)
        {
            yield return new RequiredGreaterThanZeroConstraint<ProjectModel>("Employee", m => m.EmployeeID);
            yield return new RequiredGreaterThanZeroConstraint<ProjectModel>("Customer", m => m.CustomerID);
            yield return new RequiredConstraint<ProjectModel>("Name", m => m.Name);
            yield return new RequiredGreaterThanZeroConstraint<ProjectModel>("Category", m => m.CategoryID);
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(ProjectDetailsViewModel sender, string message, ProjectModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.ProjectID == current?.ProjectID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await ProjectService.GetProjectAsync(current.ProjectID);
                                    item = item ?? new ProjectModel { ProjectID = current.ProjectID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This Project has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Project", "Handle Changes", ex);
                                }
                            });
                            break;
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async void OnListMessage(ProjectListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<ProjectModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.ProjectID == current.ProjectID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await ProjectService.GetProjectAsync(current.ProjectID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Project", "Handle Ranges Deleted", ex);
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await ContextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This Project has been deleted externally");
            });
        }
    }
}
