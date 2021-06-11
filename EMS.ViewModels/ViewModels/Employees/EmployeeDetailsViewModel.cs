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

namespace EMS.ViewModels.ViewModels.Employees
{
    #region EmployeeDetailsArgs
    public class EmployeeDetailsArgs
    {
        static public EmployeeDetailsArgs CreateDefault() => new EmployeeDetailsArgs();

        public long EmployeeID { get; set; }

        public bool IsNew => EmployeeID <= 0;
    }
    #endregion

    public class EmployeeDetailsViewModel : GenericDetailsViewModel<EmployeeModel>
    {
        public EmployeeDetailsViewModel(IEmployeeService employeeService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            EmployeeService = employeeService;
            FilePickerService = filePickerService;
        }

        public IEmployeeService EmployeeService { get; }
        public IFilePickerService FilePickerService { get; }

        override public string Title => (Item?.IsNew ?? true) ? "New Employee" : TitleEdit;
        public string TitleEdit => Item == null ? "Employee" : $"{Item.FullName}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public EmployeeDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(EmployeeDetailsArgs args)
        {
            ViewModelArgs = args ?? EmployeeDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new EmployeeModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await EmployeeService.GetEmployeeAsync(ViewModelArgs.EmployeeID);
                    Item = item ?? new EmployeeModel { EmployeeID = ViewModelArgs.EmployeeID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Employee", "Load", ex);
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.EmployeeID = Item?.EmployeeID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<EmployeeDetailsViewModel, EmployeeModel>(this, OnDetailsMessage);
            MessageService.Subscribe<EmployeeListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public EmployeeDetailsArgs CreateArgs()
        {
            return new EmployeeDetailsArgs
            {
                EmployeeID = Item?.EmployeeID ?? 0
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

        protected override async Task<bool> SaveItemAsync(EmployeeModel model)
        {
            try
            {
                StartStatusMessage("Saving Employee...");
                await Task.Delay(100);
                await EmployeeService.UpdateEmployeeAsync(model);
                EndStatusMessage("Employee saved");
                LogInformation("Employee", "Save", "Employee saved successfully", $"Employee {model.EmployeeID} '{model.FullName}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Employee: {ex.Message}");
                LogException("Employee", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(EmployeeModel model)
        {
            try
            {
                StartStatusMessage("Deleting Employee...");
                await Task.Delay(100);
                await EmployeeService.DeleteEmployeeAsync(model);
                EndStatusMessage("Employee deleted");
                LogWarning("Employee", "Delete", "Employee deleted", $"Employee {model.EmployeeID} '{model.FullName}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Employee: {ex.Message}");
                LogException("Employee", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current Employee?", "Ok", "Cancel");
        }

        override protected IEnumerable<IValidationConstraint<EmployeeModel>> GetValidationConstraints(EmployeeModel model)
        {
            yield return new RequiredConstraint<EmployeeModel>("First Name", m => m.FirstName);
            yield return new RequiredConstraint<EmployeeModel>("Last Name", m => m.LastName);
            yield return new RequiredConstraint<EmployeeModel>("Email Address", m => m.EmailAddress);
            yield return new RequiredConstraint<EmployeeModel>("Address Line 1", m => m.AddressLine1);
            yield return new RequiredConstraint<EmployeeModel>("City", m => m.City);
            yield return new RequiredConstraint<EmployeeModel>("Region", m => m.Region);
            yield return new RequiredConstraint<EmployeeModel>("Postal Code", m => m.PostalCode);
            yield return new RequiredConstraint<EmployeeModel>("Country", m => m.CountryCode);
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(EmployeeDetailsViewModel sender, string message, EmployeeModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.EmployeeID == current?.EmployeeID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await EmployeeService.GetEmployeeAsync(current.EmployeeID);
                                    item = item ?? new EmployeeModel { EmployeeID = current.EmployeeID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This Employee has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Employee", "Handle Changes", ex);
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

        private async void OnListMessage(EmployeeListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<EmployeeModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.EmployeeID == current.EmployeeID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await EmployeeService.GetEmployeeAsync(current.EmployeeID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Employee", "Handle Ranges Deleted", ex);
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
                StatusMessage("WARNING: This Employee has been deleted externally");
            });
        }
    }
}
