using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using EMS.Data.Common;
using EMS.ViewModels.Infrastructure.Common;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Models;
using EMS.ViewModels.Services;
using EMS.ViewModels.ViewModels.Common;

namespace EMS.ViewModels.ViewModels.Company
{
    #region CompanyListArgs
    public class CompanyListArgs
    {
        static public CompanyListArgs CreateEmpty() => new CompanyListArgs { IsEmpty = true };

        public CompanyListArgs()
        {
            OrderBy = r => r.CompanyID;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<Data.Data.Company, object>> OrderBy { get; set; }
        public Expression<Func<Data.Data.Company, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class CompanyListViewModel : GenericListViewModel<CompanyModel>
    {
        public CompanyListViewModel(ICompanyService companyService, ICommonServices commonServices) : base(commonServices)
        {
            CompanyService = companyService;
        }

        public ICompanyService CompanyService { get; }

        public CompanyListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(CompanyListArgs args)
        {
            ViewModelArgs = args ?? CompanyListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading Companys...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Companys loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<CompanyListViewModel>(this, OnMessage);
            MessageService.Subscribe<CompanyDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public CompanyListArgs CreateArgs()
        {
            return new CompanyListArgs
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
                Items = new List<CompanyModel>();
                StatusError($"Error loading Companys: {ex.Message}");
                LogException("Companys", "Refresh", ex);
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

        private async Task<IList<CompanyModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Data.Data.Company> request = BuildDataRequest();
                return await CompanyService.GetCompanysAsync(request);
            }
            return new List<CompanyModel>();
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await NavigationService.CreateNewViewAsync<CompanyDetailsViewModel>(new CompanyDetailsArgs { CompanyID = SelectedItem.CompanyID });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<CompanyDetailsViewModel>(new CompanyDetailsArgs());
            }
            else
            {
                NavigationService.Navigate<CompanyDetailsViewModel>(new CompanyDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading Companys...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Companys loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected Companys?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} Companys...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} Companys...");
                        await DeleteItemsAsync(SelectedItems);
                        MessageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Companys: {ex.Message}");
                    LogException("Companys", "Delete", ex);
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} Companys deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<CompanyModel> models)
        {
            foreach (var model in models)
            {
                await CompanyService.DeleteCompanyAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Data.Data.Company> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await CompanyService.DeleteCompanyRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Data.Data.Company> BuildDataRequest()
        {
            return new DataRequest<Data.Data.Company>()
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