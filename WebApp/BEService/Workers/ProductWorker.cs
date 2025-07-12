using BEService.Services;

using Shared.EF;
using Shared.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEService.Workers
{
    public class ProductWorker : BackgroundService
    {
        private readonly ILogger<ProductWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;        

        public ProductWorker(ILogger<ProductWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {                    
                    using (var context = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                    {
                        var orderRepo = scope.ServiceProvider.GetRequiredService<IHandlerService>();
                        await orderRepo.HandlerProduct();
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
