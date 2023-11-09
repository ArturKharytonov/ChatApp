using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.WebAPI.Services.MessageService.Interfaces;
using ChatApp.WebAPI.Services.QueryBuilder.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.WebAPI.Services.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryBuilder<MessageDto> _queryBuilder;
        private readonly UserManager<User> _userManager;
        private const int _pageSize = 5;
        public MessageService(IUnitOfWork unitOfWork, IQueryBuilder<MessageDto> queryBuilder, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _queryBuilder = queryBuilder;
            _userManager = userManager;
        }

        public async Task<GridModelResponse<MessageDto>> GetMessagePageAsync(GridModelDto<MessageColumnsSorting> data)
        {
            var list = await _unitOfWork.GetRepository<Message, int>()!.GetAllAsQueryableAsync();

            var taskMessages = await Task.WhenAll(list.Select(async x =>
            {
                var sender = await _userManager.FindByIdAsync(x.SenderId.ToString());
                var room = await _unitOfWork.GetRepository<Room, int>()!.GetByIdAsync(x.RoomId);

                return new MessageDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    SentAt = x.SentAt,
                    SenderUsername = sender.UserName,
                    RoomName = room.Name
                };
            }));

            var messages = taskMessages.AsQueryable();

            if (!string.IsNullOrEmpty(data.Data))
                messages = messages.Where(_queryBuilder.SearchQuery(data.Data, Enum.GetNames(data.Column.GetType())));

            if (data.Sorting)
                messages = _queryBuilder.OrderByQuery(messages, data.Column.ToString(), data.Asc);

            var messageInformation = await Task.Run(() =>
                messages
                    .Skip(data.PageNumber * _pageSize)
                    .Take(_pageSize)
                    .ToList());

            var totalCount = messages.Count();
            return new GridModelResponse<MessageDto>
            {
                Items = messageInformation,
                TotalCount = totalCount
            };
        }
    }
}
