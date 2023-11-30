using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.UI.Pages.Common.ComponentHelpers;
using ChatApp.UI.Services.UserApplicationService.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor.Rendering;
using Radzen.Blazor;
using Microsoft.JSInterop;
using ChatApp.UI.Services.SignalRService.Interfaces;
using ChatApp.UI.Services.SignalRService;

namespace ChatApp.UI.Pages.User
{
    public partial class AddUserGrid : ComponentBase,ISearchComponent
    {
        [Parameter]
        public string Id { get; set; }

        public IEnumerable<UserDto> Items { get; set; }
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public IUserApplicationService UserApplicationService { get; set; } = null!;
        [Inject] private NotificationService NotificationService { get; set; } = null!;

        [CascadingParameter]
        ISignalRService SignalRService { get; set; } = null!;

        public string? Input { get; set; }

        public async Task<GridModelResponse<UserDto>?> GetItems(int pageNumber)
        {
            var model = new GridModelDto<UserColumnsSorting>
            {
                PageNumber = pageNumber,
                Data = Input,
            };

            return await UserApplicationService.GetUsersAsync(model);
        }
        public async Task Fetch(int page)
        {
            var response = await GetItems(page);

            Items = response.Items;
            Count = response.TotalCount;
        }
        public async Task PageChanged(PagerEventArgs args)
        {
            CurrentPage = args.PageIndex;


            await Fetch(CurrentPage);
        }
        public async Task OnSearchChangeAsync(string? value)
        {
            Input = value;
            if (!string.IsNullOrEmpty(Input))
                await Fetch(CurrentPage);
            else 
                Items = new List<UserDto>();
        }
        private async Task OnRowClick(DataGridRowMouseEventArgs<UserDto> args)
        {
            var selectedUser = args.Data;

            var response = await UserApplicationService.AddUserToGroup(new AddUserToRoomDto
                { UserId = selectedUser.Id.ToString(), RoomId = Id });

            if (response.WasAdded)
            {
                await SignalRService.AddToRoom(Id);
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = $"User: {selectedUser.Id} was added to room",
                    Severity = NotificationSeverity.Info,
                    Duration = 3000,
                });
            }
            else 
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = $"Was not added, maybe user already exist",
                    Severity = NotificationSeverity.Info,
                    Duration = 3000,
                });
        }
    }
}
