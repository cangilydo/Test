using Shared.Domains;
using Shared.Enums;
using Shared.Repositories;

using System.ComponentModel;

namespace ApiService.Handler.Products
{
    [Description("2")]
    public class MonthlyProduct : IProduct
    {
        private IQueueRepository _queueRepository;
        private ILogger<MonthlyProduct> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MonthlyProduct(IServiceScopeFactory serviceProvider)
        {
            _serviceScopeFactory = serviceProvider;
        }
        public async Task Process(Guid orderId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<MonthlyProduct>>();
                _queueRepository = scope.ServiceProvider.GetRequiredService<IQueueRepository>();
                try
                {
                    await _queueRepository.AddAsync(new Queue()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        Status = (int)EQueueStatus.Create,
                        Type = (int)EQueueType.Product
                    });


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Process monthly product exception: ");
                }
            }

        }
    }
}
