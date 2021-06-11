using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EMS.ViewModels.Infrastructure.Common;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.Common;

namespace EMS.ViewModels.ViewModels.Company
{
    #region CompanyDetailsArgs
    public class CompanyDetailsArgs
    {
        static public CompanyDetailsArgs CreateDefault() => new CompanyDetailsArgs();

        public long CompanyID { get; set; }

        public bool IsNew => CompanyID <= 0;
    }
    #endregion

    public class CompanyDetailsViewModel : GenericDetailsViewModel<CompanyModel>
    {
        public CompanyDetailsViewModel(ICompanyService companyService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            CompanyService = companyService;
            FilePickerService = filePickerService;
        }

        public ICompanyService CompanyService { get; }
        public IFilePickerService FilePickerService { get; }

        override public string Title => (Item?.IsNew ?? true) ? "New Company" : TitleEdit;
        public string TitleEdit => Item == null ? "Company" : $"{Item.Name}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public CompanyDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(CompanyDetailsArgs args)
        {
            ViewModelArgs = args ?? CompanyDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new CompanyModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await CompanyService.GetCompanyAsync(ViewModelArgs.CompanyID);
                    Item = item ?? new CompanyModel { CompanyID = ViewModelArgs.CompanyID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Company", "Load", ex);
                }
            }
        }

        public void Unload()
        {
            ViewModelArgs.CompanyID = Item?.CompanyID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<CompanyDetailsViewModel, CompanyModel>(this, OnDetailsMessage);
            MessageService.Subscribe<CompanyListViewModel>(this, OnListMessage);
        }

        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public CompanyDetailsArgs CreateArgs()
        {
            return new CompanyDetailsArgs
            {
                CompanyID = Item?.CompanyID ?? 0
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
                NewPictureSource = result.ImageSource;
            }
            else
            {
                NewPictureSource = null;
            }
        }

        protected override async Task<bool> SaveItemAsync(CompanyModel model)
        {
            try
            {
                StartStatusMessage("Saving Company...");
                await Task.Delay(100);
                await CompanyService.UpdateCompanyAsync(model);
                EndStatusMessage("Company saved");
                LogInformation("Company", "Save", "Company saved successfully", $"Company {model.CompanyID} '{model.Name}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Company: {ex.Message}");
                LogException("Company", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(CompanyModel model)
        {
            try
            {
                StartStatusMessage("Deleting Company...");
                await Task.Delay(100);
                await CompanyService.DeleteCompanyAsync(model);
                EndStatusMessage("Company deleted");
                LogWarning("Company", "Delete", "Company deleted", $"Company {model.CompanyID} '{model.Name}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Company: {ex.Message}");
                LogException("Company", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current Company?", "Ok", "Cancel");
        }

        private async void OnDetailsMessage(CompanyDetailsViewModel sender, string message, CompanyModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.CompanyID == current?.CompanyID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await CompanyService.GetCompanyAsync(current.CompanyID);
                                    item = item ?? new CompanyModel { CompanyID = current.CompanyID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This Company has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Company", "Handle Changes", ex);
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

        private async void OnListMessage(CompanyListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<CompanyModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.CompanyID == current.CompanyID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await CompanyService.GetCompanyAsync(current.CompanyID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Company", "Handle Ranges Deleted", ex);
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
                StatusMessage("WARNING: This Company has been deleted externally");
            });
        }
    }
}