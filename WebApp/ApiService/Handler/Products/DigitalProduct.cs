using Microsoft.Extensions.DependencyInjection;

using Shared.Domains;
using Shared.Enums;
using Shared.Repositories;

using System;
using System.ComponentModel;

namespace ApiService.Handler.Products
{
    [Description("1")]
    public class DigitalProduct : IProduct
    {
        private IQueueRepository _queueRepository;
        private ILogger<DigitalProduct> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public DigitalProduct(IServiceScopeFactory serviceProvider)
        {
            _serviceScopeFactory = serviceProvider;
        }

        public async Task Process(Guid orderId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<DigitalProduct>>();
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

                    await _queueRepository.AddAsync(new Queue()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        Status = (int)EQueueStatus.Create,
                        Type = (int)EQueueType.Email
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Process digital product exception: ");
                }
            }
        }
    }
}
