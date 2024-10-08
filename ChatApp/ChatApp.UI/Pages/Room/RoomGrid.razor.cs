using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.UI.Pages.Common.ComponentHelpers;
using ChatApp.UI.Services.RoomApplicationService.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;


namespace ChatApp.UI.Pages.Room
{
    public partial class RoomGrid : ComponentBase, IBaseGridComponent<RoomDto>,
        ISortComponent<RoomColumnsSorting>, ISearchComponent
    {
        //base
        public IEnumerable<RoomDto> Items { get; set; }
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        //search
        public string? Input { get; set; }

        //sorting
        public bool Asc { get; set; }
        public bool Sorting { get; set; }
        public RoomColumnsSorting SortFieldValue { get; set; }
        public IEnumerable<RoomColumnsSorting> SortingFieldsDropDown { get; set; }
        [Inject] private IRoomApplicationService RoomApplicationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ReadFromUrl();
            SortingFieldsDropDown = Enum.GetValues(typeof(RoomColumnsSorting)).Cast<RoomColumnsSorting>().ToList();
            var response = await GetItems(CurrentPage);
            Items = response.Items;
            Count = response.TotalCount;
        }
        public async Task<GridModelResponse<RoomDto>?> GetItems(int pageNumber)
        {
            NavigationManager.NavigateTo($"/roomgrid?data={Input}&pageNumber={CurrentPage + 1}&column={SortFieldValue}&asc={Asc}&sorting={Sorting}");

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

            int.TryParse(queryParameters["pageNumber"], out var value);
            if (value > 0)
                CurrentPage = value - 1;

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
        private void OnRowClick(DataGridRowMouseEventArgs<RoomDto> args)
        {
            var selectedRoom = args.Data;

            NavigationManager.NavigateTo($"/chat/{selectedRoom.Id}");
        }
        private void ShowInlineDialog()
        {
            NavigationManager.NavigateTo("/createroom");
        }
    }
}

