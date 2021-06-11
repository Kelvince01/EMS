using System;
using System.Threading.Tasks;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;

namespace EMS.ViewModels.ViewModels.Company
{
    public class CompanyViewModel : ViewModelBase
    {
        public CompanyViewModel(ICompanyService companyService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            CompanyService = companyService;

            CompanyList = new CompanyListViewModel(CompanyService, commonServices);
            CompanyDetails = new CompanyDetailsViewModel(CompanyService, filePickerService, commonServices);
        }

        public ICompanyService CompanyService { get; }
        public CompanyListViewModel CompanyList { get; set; }

        public CompanyDetailsViewModel CompanyDetails { get; set; }

        public async Task LoadAsync(CompanyListArgs args)
        {
            await CompanyList.LoadAsync(args);
        }

        public void Unload()
        {
            CompanyDetails.CancelEdit();
            CompanyList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<CompanyListViewModel>(this, OnMessage);
            CompanyList.Subscribe();
            CompanyDetails.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            CompanyList.Unsubscribe();
            CompanyDetails.Unsubscribe();
        }

        private async void OnMessage(CompanyListViewModel viewModel, string message, object args)
        {
            if (viewModel == CompanyList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (CompanyDetails.IsEditMode)
            {
                StatusReady();
                CompanyDetails.CancelEdit();
            }
            var selected = CompanyList.SelectedItem;
            if (!CompanyList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            CompanyDetails.Item = selected;
        }

        private async Task PopulateDetails(CompanyModel selected)
        {
            try
            {
                var model = await CompanyService.GetCompanyAsync(selected.CompanyID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Companys", "Load Details", ex);
            }
        }
    }
}