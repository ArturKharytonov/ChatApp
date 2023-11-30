using ChatApp.Application.Services.MessageService.Interfaces;
using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services.MessageService
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

        public async Task<IEnumerable<MessageDto>> GetMessagesFromChat(string roomId)
        {
            var roomsQuery = await _unitOfWork.GetRepository<Room, int>()!.GetAllAsQueryableAsync();

            var room = roomsQuery
                .Include(r => r.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefault(r => r.Id == int.Parse(roomId));

            var messages = room!.Messages
                .Select(x => new MessageDto
            {
                Id = x.Id,
                Content = x.Content,
                SentAt = x.SentAt,
                RoomName = x.Room.Name,
                SenderUsername = x.Sender.UserName!
            });

            return messages;
        }
        public async Task<MessageDto> AddMessageAsync(AddMessageDto addMessageDto)
        {
            var user = await _userManager.FindByIdAsync(addMessageDto.UserId);
            var room = await _unitOfWork.GetRepository<Room, int>()!.GetByIdAsync(addMessageDto.RoomId);

            var message = new Message
            {
                Content = addMessageDto.Content,
                SentAt = addMessageDto.SentAt,
                RoomId = room.Id,
                SenderId = user.Id,
            };

            await _unitOfWork.GetRepository<Message, int>()!.CreateAsync(message);
            await _unitOfWork.SaveAsync();

            return new MessageDto
            {
                Content = addMessageDto.Content,
                SentAt = addMessageDto.SentAt,
                SenderUsername = user.UserName,
                RoomName = room.Name
            };
        }
        public async Task<GridModelResponse<MessageDto>> GetMessagePageAsync(GridModelDto<MessageColumnsSorting> data)
        {
            var list = await _unitOfWork.GetRepository<Message, int>()!.GetAllAsQueryableAsync();

            var messages = list.AsEnumerable()
                .Select(async x => new MessageDto
            {
                Id = x.Id,
                Content = x.Content,
                SentAt = x.SentAt,
                SenderUsername = (await _userManager.FindByIdAsync(x.SenderId.ToString()))?.UserName!,
                RoomName = (await _unitOfWork.GetRepository<Room, int>()!.GetByIdAsync(x.RoomId))?.Name!
            }).Select(t => t.Result).AsQueryable();

            if (!string.IsNullOrEmpty(data.Data))
                messages = messages.Where(_queryBuilder.SearchQuery(data.Data, Enum.GetNames(data.Column.GetType())));

            if (data.Sorting)
                messages = _queryBuilder.OrderByQuery(messages, data.Column.ToString(), data.Asc);

            var messageInformation = messages
                .Skip(data.PageNumber * _pageSize)
                .Take(_pageSize)
                .ToList();

            var totalCount = messages.Count();
            return new GridModelResponse<MessageDto>
            {
                Items = messageInformation,
                TotalCount = totalCount
            };
        } 
        public async Task<bool> UpdateMessageAsync(MessageDto message)
        {
            var repo = _unitOfWork.GetRepository<Message, int>();
            var updatingMessage = await repo.GetByIdAsync(message.Id);

            if (updatingMessage == null) return false;

            updatingMessage.Content = message.Content;
            repo.Update(updatingMessage);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task DeleteMessageAsync(int messageId)
        {
            var repo = _unitOfWork.GetRepository<Message, int>()!;
            await repo.DeleteAsync(messageId);
            await _unitOfWork.SaveAsync();
        }
    }
}
