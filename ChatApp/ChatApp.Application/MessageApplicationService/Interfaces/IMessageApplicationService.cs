using ChatApp.Domain.DTOs.RoomDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.MessageApplicationService.Interfaces
{
    public interface IMessageApplicationService
    {
        Task<List<MessageDto>> GetMessagesAsync(string roomId);
        Task<MessageDto> AddMessageAsync(AddMessageDto message);
        Task<GridModelResponse<MessageDto>> GetMessagesAsync(GridModelDto<MessageColumnsSorting> gridModelDto);
    }
}
