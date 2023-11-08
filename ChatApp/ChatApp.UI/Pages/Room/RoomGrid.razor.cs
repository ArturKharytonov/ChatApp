using ChatApp.Application.RoomApplicationService.Interfaces;
using ChatApp.Application.UserApplicationService;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ChatApp.UI.Pages.Room
{
    public partial class RoomGrid : IBaseGridComponent<RoomDto, RoomColumnsSorting>
    {
        private IEnumerable<RoomDto> Items;
        private int Count;
        private string? Input;
        private int CurrentPage;

        private bool Asc { get; set; }
        private bool Sorting { get; set; }
        private RoomColumnsSorting SortFieldValue { get; set; }

        public IEnumerable<RoomColumnsSorting> SortingFieldsDropDown { get; set; }

        [Inject] public IRoomApplicationService RoomApplicationService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ReadFromUrl();

            SortingFieldsDropDown = Enum.GetValues(typeof(RoomColumnsSorting)).Cast<RoomColumnsSorting>().ToList();

            var response = await GetItems(CurrentPage);

            if (response == null)
            {
                NavigationManager.NavigateTo("/logout");

                StateHasChanged();
            }

            else
            {
                Items = response.Items;
                Count = response.TotalCount;
            }
        }

        public async Task<GridModelResponse<RoomDto>?> GetItems(int pageNumber)
        {
            NavigationManager.NavigateTo($"/rootgrid?data={Input}&pageNumber={CurrentPage + 1}&column={SortFieldValue}&asc={Asc}&sorting={Sorting}");

            var model = new GridModelDto<RoomColumnsSorting>
            {
                PageNumber = pageNumber,
                Data = Input,
                Sorting = Sorting
            };

            if (Sorting)
            {
                model.Column = SortFieldValue;
                model.Asc = Asc;
            }

            return await RoomApplicationService.GetRoomsAsync(model);
        }

        public async Task PageChanged(PagerEventArgs args)
        {
            CurrentPage = args.PageIndex;


            await Fetch(CurrentPage);
        }

        public async Task OnSearchChangeAsync(string? value)
        {
            Input = value;

            await Fetch(CurrentPage);
        }

        public async Task OnSortChangeAsync()
        {
            await Fetch(CurrentPage);
        }

        public void ReadFromUrl()
        {
            var queryString = new Uri(NavigationManager.Uri).Query;
            var queryParameters = System.Web.HttpUtility.ParseQueryString(queryString);
            if (queryParameters.Count <= 0) return;

            int.TryParse(queryParameters["pageNumber"], out CurrentPage);
            if (CurrentPage > 0)
                CurrentPage -= 1;

            Input = queryParameters["data"];

            Enum.TryParse(queryParameters["column"], out RoomColumnsSorting read);
            SortFieldValue = read;

            bool.TryParse(queryParameters["asc"], out var asc);
            Asc = asc;

            bool.TryParse(queryParameters["sorting"], out var sorting);
            Sorting = sorting;
        }

        public async Task Fetch(int page)
        {
            var response = await GetItems(page);

            Items = response.Items;
            Count = response.TotalCount;
        }
    }
}
