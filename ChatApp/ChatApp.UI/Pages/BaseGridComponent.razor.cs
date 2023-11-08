using ChatApp.Application.UserApplicationService.Interfaces;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ChatApp.UI.Pages
{
    public interface IBaseGridComponent<TItem, TSortingEnum>
    {
        IEnumerable<TSortingEnum> SortingFieldsDropDown { get; set; }

        NavigationManager NavigationManager { get; set; }

        Task<GridModelResponse<TItem>?> GetItems(int pageNumber);

        Task PageChanged(PagerEventArgs args);

        Task OnSearchChangeAsync(string? value);

        Task OnSortChangeAsync();

        void ReadFromUrl();

        Task Fetch(int page);
    }
}
