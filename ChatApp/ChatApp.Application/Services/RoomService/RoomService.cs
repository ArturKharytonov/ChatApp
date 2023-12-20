using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Domain.DTOs.FileDto;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using File = ChatApp.Domain.Files.File;

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

    public async Task<RoomDto> GetRoomByName(string name)
    {
        var rooms = await _unitOfWork.GetRepository<Room, int>()!.GetAllAsQueryableAsync();
        var room = await rooms.FirstOrDefaultAsync(x => x.Name.Equals(name));
        
        if (room == null)
            return new RoomDto();

        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            AssistantId = room.AssistantId,
        };
    }
    public async Task<RoomDto> GetRoom(int id)
    {
        var room = await _unitOfWork.GetRepository<Room, int>()!.GetByIdAsync(id);
        var files = await _unitOfWork.GetRepository<File, string>()!.GetAllAsQueryableAsync();
        files = files.Where(f => f.GroupId == id);

        if (room == null)
            return new RoomDto();

        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            AssistantId = room.AssistantId,
            Files = GetFiles(files)
        };
    }
    public async Task<int?> CreateRoom(string name, string creatorId, string assistantId)
    {
        var repo = _unitOfWork.GetRepository<Room, int>()!;
        var list = await repo.GetAllAsQueryableAsync();
        var user = await _userManager.FindByIdAsync(creatorId);

        if (list.Any(x => x.Name == name) ||
            user == null) 
            return null;

        var room = new Room { Name = name, AssistantId = assistantId };

        room.Users.Add(user);
            
        await repo.CreateAsync(room);
        await _unitOfWork.SaveAsync();

        return room.Id;
    }
    public async Task<GridModelResponse<RoomDto>> GetRoomsPageAsync(int userId, GridModelDto<RoomColumnsSorting> data)
    {
        var list = await _unitOfWork.GetRepository<Room, int>()!.GetAllAsQueryableAsync();

        list = list.Where(room => room.Users.Any(user => user.Id == userId));

        var rooms = list.Select(x => new RoomDto
        {
            Id = x.Id,
            Name = x.Name,
            ParticipantsNumber = x.Users.Count,
            MessagesNumber = x.Messages.Count,
            AssistantId = x.AssistantId
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
    private List<FileDto> GetFiles(IQueryable<File> files)
    {
        return files.Select(file =>
            new FileDto { Id = file.Id, RoomName = file.Group.Name, Username = file.User.UserName}).ToList();
    }
}