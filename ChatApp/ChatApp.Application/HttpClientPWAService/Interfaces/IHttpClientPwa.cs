using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.HttpClientPWAService.Interfaces
{
    public interface IHttpClientPwa
    {
        public Task<ApiRequestResult<TResult>> PostAsync<TArgument, TResult>(string requestUrl, TArgument data);
    }
}
