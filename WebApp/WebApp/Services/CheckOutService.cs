using MediatR;

using Newtonsoft.Json;

using Shared.Domains;
using Shared.Dto;
using Shared.Requests.Commands;
using Shared.Requests.Queries;

using System;

namespace WebApp.Services
{
    public interface ICheckOutService
    {
        Task<PagedResult<OrderDetailDto>> GetList(SearchQuery request);
        Task<BaseRes<string>> CheckOut(Dictionary<Guid, Guid> input);
    }
    public class CheckOutService : ICheckOutService
    {
        private readonly HttpClient _client;
        private readonly ILogger<CheckOutService> _logger;
        public CheckOutService(IHttpClientFactory httpClientFactory, ILogger<CheckOutService> logger)
        {
            _client = httpClientFactory.CreateClient("InternalHost");
            _logger = logger;
        }

        public async Task<PagedResult<OrderDetailDto>> GetList(SearchQuery request)
        {
            try
            {
                var res = await _client.PostAsJsonAsync<SearchQuery>("CheckOut/list", request);
                if (res != null && res.IsSuccessStatusCode)
                {
                    var content = await res.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<BaseRes<PagedResult<OrderDetailDto>>>(content);
                    if (dto != null && dto.Status == Shared.Enums.EResStatus.Success)
                        return dto.Data;

                    return null;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetList exception: ");
                return null;
            }
        }

        public async Task<BaseRes<string>> CheckOut(Dictionary<Guid, Guid> input)
        {
            try
            {
                var res = await _client.PostAsJsonAsync<CheckOutCmd>("CheckOut/process", new CheckOutCmd()
                {
                    OrderIdPair = input
                });
                if (res != null && res.IsSuccessStatusCode)
                {
                    var content = await res.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<BaseRes<string>>(content);

                    return dto;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckOut exception: ");
                return null;
            }
        }
    }
}
