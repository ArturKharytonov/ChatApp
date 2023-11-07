using System.Net;
using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using Microsoft.AspNetCore.Components;
using Radzen;
using ChatApp.Domain.Enums;

namespace ChatApp.UI.Pages
{
    public partial class UserGrid
    {
        private IEnumerable<UserDto> _users;
        private int _count;
        private string? _input;
        private int _currentPage = 1;

        protected UserColumnsSorting SortFieldValue { get; set; }
        protected bool _asc { get; set; }
        protected bool _sorting { get; set; }

        protected IEnumerable<UserColumnsSorting> SortingFieldsDropDown { get; set; }

        [Inject] private IUserCredentialsService UserCredentialsService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }


        protected override async Task OnInitializedAsync()
        {
            _sorting = false;
            _asc = true;
            SortingFieldsDropDown = Enum.GetValues(typeof(UserColumnsSorting)).Cast<UserColumnsSorting>().ToList();

            var response = await GetUsers(1);

            if (response == null)
            {
                NavigationManager.NavigateTo("/logout");

                StateHasChanged();
            }

            else
            {

                _users = response.Users;
                _count = response.TotalCount;
            }
        }


        async Task<GridModelResponse<UserDto>?> GetUsers(int pageNumber)
        {
            var model = new GridModelDto<UserColumnsSorting>
            {
                PageNumber = pageNumber,
                Data = _input,
                Sorting = _sorting
            };

            if (_sorting)
            {
                model.Column = SortFieldValue;
                model.Asc = _asc;
            }
            return await UserCredentialsService.GetUsersAsync(model);
        }

        async Task PageChanged(PagerEventArgs args)
        {
            _currentPage = args.PageIndex + 1;

            var response = await GetUsers(_currentPage);
            _users = response.Users;
        }

        async Task OnSearchChangeAsync(string? value)
        {
            _input = value;

            var response = await GetUsers(1);
            _users = response.Users;
            _count = response.TotalCount;
        }

        async Task OnSortChangeAsync()
        {
            var response = await GetUsers(_currentPage);

            _users = response.Users;
            _count = response.TotalCount;
        }
    }
}
