using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace ChatApp.UI.Pages
{
    public partial class UserGrid
    {
        private IEnumerable<UserDto> _users;
        private int _count;
        private string? _input;

        [Inject] private IUserCredentialsService UserCredentialsService { get; set; }

        async Task PageChanged(PagerEventArgs args)
        {
            var response = await GetUsers(args.PageIndex + 1);
            _users = response.Users;
        }

        async Task<GridModelResponse> GetUsers(int pageNumber)
            => await UserCredentialsService.GetUsersAsync(new GridModelDto
            {
                PageNumber = pageNumber,
                Data = _input
            });

        async Task OnChange(string? value)
        {
            _input = value;
            if (!string.IsNullOrEmpty(_input))
            {
                var response = await GetUsers(1);
                _users = response.Users;
                _count = response.TotalCount;
            }
            else
            {
                // get all users
                _count = 0;
                _users = new List<UserDto>();
            }
        }
    }
}
