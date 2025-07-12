using ApiService.Handler.Products;

using Shared.Domains;
using Shared.Dto;
using Shared.Enums;
using Shared.Repositories;
using Shared.Requests.Commands;

using System.Net.Http;

namespace ApiService.Services
{
    public interface ICheckOutService
    {
        Task<string> AuditCheck(CheckOutCmd request);
        Task<string> CallPayment();
        List<Order> GetOrderByIds(List<Guid> ids);
        Task<bool> UpdateOrder(List<Order> input);
        void HandleProcess(List<Order> inputs);

        Task<PagedResult<V_OrderDetail>> SearchPaging(string searchText, int pageIndex, int pageSize);
    }
    public class CheckOutService : ICheckOutService
    {
        private readonly HttpClient _client;
        private readonly IAuditRepository _auditRepository;
        private readonly IServiceScopeFactory _serviceProvider;
        private readonly IOrderRepository _orderRepository;
        private readonly IV_OrderDetailRepository _V_OrderDetailRepository;
        private readonly ProductGenerate _productGenerate;
        public CheckOutService(IHttpClientFactory httpClientFactory,
            IAuditRepository auditRepository,            
            IOrderRepository orderRepository,
            IServiceScopeFactory serviceProvider,
            IV_OrderDetailRepository V_OrderDetailRepository,
            ProductGenerate productGenerate)
        {
            _client = httpClientFactory.CreateClient("ExternalApi");
            _serviceProvider = serviceProvider;
            _auditRepository = auditRepository;            
            _orderRepository = orderRepository;
            _productGenerate = productGenerate;
            _V_OrderDetailRepository = V_OrderDetailRepository;
        }

        public async Task<string> AuditCheck(CheckOutCmd request)
        {
            foreach (var s in request.OrderIdPair)
            {
                if (_auditRepository.Any(x => x.OrderId == s.Value && x.Status == (int)EAudit.CheckOut))
                {
                    return "confict";
                }

                await _auditRepository.AddAsync(new Audit()
                {
                    Id = s.Key,
                    OrderId = s.Value,
                    Status = (int)EAudit.CheckOut,
                });
            };
            return string.Empty;
        }

        public async Task<string> CallPayment()
        {
            var random = new Random();

            var res = await _client.PostAsJsonAsync<string>("External/payment", random.Next(0, 1000000).ToString());

            if (res != null && res.IsSuccessStatusCode)
            {
                return string.Empty;
            }
            else
            {
                return "payment fail";
            }
        }

        public List<Order> GetOrderByIds(List<Guid> ids)
        {
            return _orderRepository.Find(s => ids.Contains(s.Id)).ToList();
        }

        public async Task<bool> UpdateOrder(List<Order> input)
        {

            foreach (var item in input)
            {
                await _orderRepository.UpdateAsync(item);
            }

            
            return true;
        }

        public void HandleProcess(List<Order> inputs)
        {
            inputs.ForEach(s =>
            {
                if (s.Type != null)
                {
                    var productInstance = _productGenerate.CreateProduct(s.Type.Value.ToString(), _serviceProvider);
                    productInstance.Process(s.Id);
                }

            });
        }

        public async Task<PagedResult<V_OrderDetail>> SearchPaging(string searchText, int pageIndex, int pageSize)
        {
            var res = await _V_OrderDetailRepository.SearchPaging(searchText, pageIndex, pageSize);
            return res;
        }
    }
}
