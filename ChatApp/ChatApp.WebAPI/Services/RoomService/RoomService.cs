using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.WebAPI.Services.QueryBuilder.Interfaces;
using ChatApp.WebAPI.Services.RoomService.Interfaces;

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
            var list = await _unitOfWork.GetRepository<Room, int>()!.GetAllAsQueryableAsync();

            var rooms = list.Select(x => new RoomDto
            {
                Id = x.Id,
                Name = x.Name,
                ParticipantsNumber = x.Users.Count,
                MessagesNumber = x.Messages.Count
            });

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
