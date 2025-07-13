using Microsoft.AspNetCore.Components;

using Shared.Domains;
using Shared.Dto;
using Shared.Requests.Queries;

using System.Collections.Generic;

using WebApp.Services;

namespace WebApp.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject]
        protected ICheckOutService service { get; set; }
        protected PagedResult<OrderDetailDto> lst { get; set; }
        protected bool selectAll = false;
        protected int PageIndex = 1;
        protected int PageSize = 1;
        protected string SearchText = string.Empty;
        protected SearchQuery searchQuery = new SearchQuery();
        protected bool isPopupVisible = false;
        protected string Msg = string.Empty;
        protected string Title = string.Empty;
        protected int PopupType = 0; //0:error, 1:confirm, 2:success
        protected bool IsLoading = false;
        protected Dictionary<Guid, Guid> SelectedItems = new Dictionary<Guid, Guid>();
        protected override async Task OnInitializedAsync()
        {
            await ReloadLst();
        }

        protected async Task ReloadLst()
        {
            searchQuery.PageIndex = PageIndex;
            searchQuery.PageSize = PageSize;
            lst = await service.GetList(searchQuery);
            StateHasChanged();
        }

        protected async void HandleValidSubmit()
        {
            PageIndex = 1;
            await ReloadLst();
        }

        protected bool CanGoToPreviousPage => PageIndex > 1;
        protected bool CanGoToNextPage => PageIndex * PageSize < lst.Count;



        protected async Task PreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                PageIndex--;
                selectAll = false;
                await ReloadLst();
                
            }
        }

        protected async Task NextPage()
        {
            if (CanGoToNextPage)
            {
                PageIndex++;
                selectAll = false;
                await ReloadLst();
                
            }
        }
        protected void OnSelectItem(ChangeEventArgs e, Guid orderId)
        {
            var checkedValue = (bool)e.Value;
            if (checkedValue && !SelectedItems.Any(s => s.Value == orderId))
                SelectedItems.Add(Guid.NewGuid(), orderId);
            else
            {
                var removeIds = SelectedItems.Where(s => s.Value == orderId);
                if (removeIds.Count() > 0)
                {
                    SelectedItems.Remove(removeIds.First().Key);
                }
            }
        }
        protected void OnSelectAllChanged(ChangeEventArgs e)
        {
            selectAll = (bool)e.Value;

            foreach (var product in lst.Result)
            {
                product.IsSelected = selectAll;
                if (selectAll && !SelectedItems.Any(s => s.Value == product.OrderId))
                    SelectedItems.Add(Guid.NewGuid(), product.OrderId);
            }

            if (!selectAll)
                SelectedItems = new Dictionary<Guid, Guid>();

            StateHasChanged();
        }
        protected void HandleCheckOut()
        {
            if (!SelectedItems.Any())
            {
                ShowPopup("Please select at least one item to check out!", "Error", 0);
                return;
            }
            else
            {
                ShowPopup("Are you sure you want to check out the selected items?", "Confirm", 1);
                return;
            }
        }

        protected async Task ConfirmCheckOut()
        {
            try
            {
                IsLoading = true;
                var res = await service.CheckOut(SelectedItems);
                if (res == null)
                {
                    ShowPopup("An unexpected error occurred", "Error", 0);
                    return;
                }

                if (res.Status == Shared.Enums.EResStatus.ValidateError)
                {
                    ShowPopup(res.Data, "Error", 1);
                    return;
                }

                if (res.Status == Shared.Enums.EResStatus.SystemError)
                {
                    ShowPopup("An error has occurred while processing your request", "Error", 1);
                    return;
                }

                if (res.Status == Shared.Enums.EResStatus.Success)
                {
                    ShowPopup("Payment successful. Your request is being processed", "Success", 2);
                    return;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
        protected void ShowPopup(string msg, string title, int type)
        {
            isPopupVisible = true;
            Title = title;
            PopupType = type;
            Msg = msg;
        }
    }
}
