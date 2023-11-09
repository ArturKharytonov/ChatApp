using ChatApp.Application.UserApplicationService.Interfaces;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ChatApp.UI.Pages.Common.ComponentHelpers
{
    public interface IBaseGridComponent<TItem>
    {
        protected IEnumerable<TItem> Items { get; set; }
        protected int Count { get; set; }
        protected int CurrentPage { get; set; }
        protected NavigationManager NavigationManager { get; set; }


        Task<GridModelResponse<TItem>?> GetItems(int pageNumber);

        Task PageChanged(PagerEventArgs args);

        void ReadFromUrl();

        Task Fetch(int page);
    }
}
