using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.DTOs.MessageDto;

namespace ChatApp.Application.MessageApplicationService.Interfaces
{
    public interface IMessageApplicationService
    {
        Task<GridModelResponse<MessageDto>> GetMessagesAsync(GridModelDto<MessageColumnsSorting> gridModelDto);
    }
}
