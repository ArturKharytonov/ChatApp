using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;

namespace ChatApp.Application.HttpClientPWAService.Interfaces
{
    public interface IHttpClientPwa
    {
        Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data, bool withJwt = false);
    }
}
