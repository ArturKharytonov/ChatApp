using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.WebAPI.Services.QueryBuilder.Interfaces;
using ChatApp.WebAPI.Services.RoomService.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.WebAPI.Services.RoomService
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryBuilder<Room> _queryBuilder;
        private const int _pageSize = 5;

        public RoomService(IUnitOfWork unitOfWork, IQueryBuilder<Room> queryBuilder)
        {
            _unitOfWork = unitOfWork;
            _queryBuilder = queryBuilder;
        }

        public GridModelResponse<RoomDto> GetRoomsPage(GridModelDto<RoomColumnsSorting> data)
        {
            var rooms = new List<Room>().AsQueryable(); //_unitOfWork.GetRepository<Room>(); // i need to get all users

            if (!string.IsNullOrEmpty(data.Data))
                rooms = rooms.Where(_queryBuilder.SearchQuery(data.Data, Enum.GetNames(data.Column.GetType())));

            if (data.Sorting)
                rooms = _queryBuilder.OrderByQuery(rooms, data.Column.ToString(), data.Asc);

            var roomInformation = rooms
                .Skip(data.PageNumber * _pageSize)
                .Take(_pageSize)
                .Select(room => new RoomDto
                {
                    Name = room.Name,
                }).ToList();

            var totalCount = rooms.Count();


            return new GridModelResponse<RoomDto>
            {
                Items = roomInformation,
                TotalCount = totalCount
            };
        }
    }
}
