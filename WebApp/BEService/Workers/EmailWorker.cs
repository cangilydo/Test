using BEService.Services;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using Shared.EF;
using Shared.Repositories;

namespace BEService.Workers
{
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EmailWorker(ILogger<EmailWorker> logger, IServiceScopeFactory serviceScopeFactory)
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
                    var orderRepo = scope.ServiceProvider.GetRequiredService<IHandlerService>();
                    await orderRepo.HandleEmail();
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
