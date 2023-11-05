using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;

namespace ChatApp.Application.HttpClientPWAService.Interfaces
{
    public interface IHttpClientPwa
    {
        Task<ApiRequestResult<T>> GetAsync<T>(string requestUrl, GridModelDto? data = null);
        Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data);
    }
}
