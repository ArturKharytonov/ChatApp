using ChatApp.Application.MessageApplicationService.Interfaces;
using ChatApp.Application.RoomApplicationService;
using ChatApp.Application.RoomApplicationService.Interfaces;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.UI.Pages.Common.ComponentHelpers;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ChatApp.UI.Pages.Message
{
    public partial class MessageGrid : ComponentBase, IBaseGridComponent<MessageDto>,
    ISortComponent<MessageColumnsSorting>, ISearchComponent
    {
        //base
        public IEnumerable<MessageDto> Items { get; set; }
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        //search
        public string? Input { get; set; }

        //sorting
        public bool Asc { get; set; }
        public bool Sorting { get; set; }
        public MessageColumnsSorting SortFieldValue { get; set; }
        public IEnumerable<MessageColumnsSorting> SortingFieldsDropDown { get; set; }

        [Inject] public IMessageApplicationService MessageApplicationService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            ReadFromUrl();

            SortingFieldsDropDown = Enum.GetValues(typeof(MessageColumnsSorting)).Cast<MessageColumnsSorting>().ToList();

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

        public async Task<GridModelResponse<MessageDto>?> GetItems(int pageNumber)
        {
            NavigationManager.NavigateTo($"/messagegrid?data={Input}&pageNumber={CurrentPage + 1}&column={SortFieldValue}&asc={Asc}&sorting={Sorting}");

            var model = new GridModelDto<MessageColumnsSorting>
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

            return await MessageApplicationService.GetMessagesAsync(model);
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

            Enum.TryParse(queryParameters["column"], out MessageColumnsSorting read);
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
