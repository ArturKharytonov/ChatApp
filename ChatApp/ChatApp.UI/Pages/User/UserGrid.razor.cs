using ChatApp.Application.UserApplicationService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using Microsoft.AspNetCore.Components;
using Radzen;
using ChatApp.Domain.Enums;

namespace ChatApp.UI.Pages.User
{
    public partial class UserGrid : IBaseGridComponent<UserDto, UserColumnsSorting>
    {
        private IEnumerable<UserDto> Items;
        private int Count;
        private string? Input;
        private int CurrentPage;
        private UserColumnsSorting SortFieldValue { get; set; }
        private bool Asc { get; set; }
        private bool Sorting { get; set; }


        public IEnumerable<UserColumnsSorting> SortingFieldsDropDown { get; set; }

        [Inject] private IUserApplicationService UserApplicationService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }


        protected override async Task OnInitializedAsync()
        {
            ReadFromUrl();

            SortingFieldsDropDown = Enum.GetValues(typeof(UserColumnsSorting)).Cast<UserColumnsSorting>().ToList();

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

        public async Task<GridModelResponse<UserDto>?> GetItems(int pageNumber)
        {
            NavigationManager.NavigateTo($"/grid?data={Input}&pageNumber={CurrentPage + 1}&column={SortFieldValue}&asc={Asc}&sorting={Sorting}");

            var model = new GridModelDto<UserColumnsSorting>
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
            return await UserApplicationService.GetUsersAsync(model);
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

            Enum.TryParse(queryParameters["column"], out UserColumnsSorting read);
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
