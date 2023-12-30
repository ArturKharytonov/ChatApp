using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Mappers.Rooms;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services.RoomService;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQueryBuilder<RoomDto> _queryBuilder;
    private readonly UserManager<User> _userManager;
    private const int _pageSize = 5;
    public RoomService(IQueryBuilder<RoomDto> queryBuilder, IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _queryBuilder = queryBuilder;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task DeleteRoom(int roomId)
    {
        var repo = _unitOfWork.GetRepository<Room, int>()!;
        await repo.DeleteAsync(roomId);
        await _unitOfWork.SaveAsync();
    }
    public async Task<RoomDto> GetRoomByName(string name)
    {
        var rooms = await _unitOfWork
            .GetRepository<Room, int>()!
            .GetAllAsQueryableAsync();
        rooms = rooms
            .Include(r => r.Files)
            .Include(r => r.Messages)
            .Include(r => r.Users);
        var room = await rooms.FirstOrDefaultAsync(x => x.Name.Equals(name));

        return room == null ? new RoomDto() : room.ToChatDto();
    }
    public async Task<RoomDto> GetRoom(int id)
    {
        var rooms = await _unitOfWork
            .GetRepository<Room, int>()!
            .GetAllAsQueryableAsync();

        var room = await rooms
            .Include(r => r.Files)
            .Include(r => r.Messages)
            .Include(r => r.Users)
            .FirstOrDefaultAsync(x => x.Id == id);

        return room == null ? new RoomDto() : room.ToChatDto();
    }
    public async Task<bool> DoesRoomExist(string roomName)
    {
        var list = await _unitOfWork.GetRepository<Room, int>()!.GetAllAsQueryableAsync();
        return list.Any(x => x.Name == roomName);
    }
    public async Task<int?> CreateRoom(string name, string creatorId, string assistantId)
    {
        var repo = _unitOfWork.GetRepository<Room, int>()!;
        var user = await _userManager.FindByIdAsync(creatorId);

        if (user == null) 
            return null;

        var room = new Room { Name = name, AssistantId = assistantId, CreatorId = user.Id};

        room.Users.Add(user);

        await repo.CreateAsync(room);
        await _unitOfWork.SaveAsync();

        return room.Id;
    }
    public async Task<GridModelResponse<RoomDto>> GetRoomsPageAsync(int userId, GridModelDto<RoomColumnsSorting> data)
    {
        var list = await _unitOfWork
            .GetRepository<Room, int>()!
            .GetAllAsQueryableAsync();
        list = list
            .Include(r => r.Users)
            .Include(r => r.Messages)
            .Include(r => r.Files)
            .Where(room => room.Users.Any(user => user.Id == userId));

        var rooms = list.Select(x => x.ToChatDto());

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