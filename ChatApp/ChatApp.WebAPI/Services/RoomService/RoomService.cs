using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.UsersAndRooms;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.WebAPI.Services.QueryBuilder.Interfaces;
using ChatApp.WebAPI.Services.RoomService.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.WebAPI.Services.RoomService
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryBuilder<RoomDto> _queryBuilder;
        private const int _pageSize = 5;

        public RoomService(IQueryBuilder<RoomDto> queryBuilder, IUnitOfWork unitOfWork)
        {
            _queryBuilder = queryBuilder;
            _unitOfWork = unitOfWork;
        }

        public async Task<GridModelResponse<RoomDto>> GetRoomsPageAsync(GridModelDto<RoomColumnsSorting> data)
        {
            var list = await _unitOfWork.GetRepository<Room>()!.GetAllAsync();

            var roomsTasks = list.Select(async x => new RoomDto
            {
                Id = x.Id,
                Name = x.Name,
                ParticipantsNumber = x.UsersAndRooms.Count,
                MessagesNumber = x.Messages.Count
            });
            var roomsList = await Task.WhenAll(roomsTasks);
            var rooms = roomsList.AsQueryable();

            if (!string.IsNullOrEmpty(data.Data))
                rooms = rooms.Where(_queryBuilder.SearchQuery(data.Data, Enum.GetNames(data.Column.GetType())));

            if (data.Sorting)
                rooms = _queryBuilder.OrderByQuery(rooms, data.Column.ToString(), data.Asc);

            var roomInformation = rooms
                .Skip(data.PageNumber * _pageSize)
                .Take(_pageSize)
                .ToList();

            var totalCount = rooms.Count();

            return await Task.FromResult(new GridModelResponse<RoomDto>
            {
                Items = roomInformation,
                TotalCount = totalCount
            });
        }
    }
}
