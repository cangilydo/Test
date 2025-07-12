using Microsoft.Extensions.Logging;

using Shared.Domains;
using Shared.EF;
using Shared.Enums;
using Shared.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BEService.Services
{
    public interface IHandlerService
    {
        Task HandleEmail();
        Task HandlerProduct();
    }
    public class HandlerService : IHandlerService
    {
        private readonly IQueueRepository _queueRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HandlerService> _logger;
        private readonly HttpClient _client;
        public HandlerService(IQueueRepository queueRepository, IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory, ILogger<HandlerService> logger)
        {
            _queueRepository = queueRepository;
            _client = httpClientFactory.CreateClient("ExternalApi");
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task HandleEmail()
        {
            try
            {
                var items = _queueRepository.Find(s => s.Type == (int)EQueueType.Email && (s.Status == (int)EQueueStatus.Create || s.Status == (int)EQueueStatus.Error)).ToList();

                var batchSize = 100;

                for (int i = 0; i < items.Count; i += batchSize)
                {
                    var batchItem = items.GetRange(i, Math.Min(batchSize, items.Count - i));
                    var taskLst = new List<Task>();
                    batchItem.ForEach(s =>
                    {
                        taskLst.Add(ProcessTask(s));
                    });
                    await Task.WhenAll(taskLst);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleEmail exception: ");
            }

            async Task ProcessTask(Queue s)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                    {
                        try
                        {
                            context.Database.BeginTransaction();

                            s.Status = (int)EQueueStatus.Processing;
                            context.Entry(s).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            context.SaveChanges();

                            var orderInfo = context.Order.FirstOrDefault(x => x.Id == s.OrderId);

                            var res = await _client.PostAsJsonAsync<string>("email", s.Id.ToString());
                            if (res != null && res.IsSuccessStatusCode)
                            {
                                s.Status = (int)EQueueStatus.Complete;
                                orderInfo.Status = (int)EOrderStatus.Email;
                            }
                            else
                            {
                                s.RetryTime = s.RetryTime == null ? 1 : s.RetryTime++;
                                s.Status = s.RetryTime > 3 ? (int)EQueueStatus.FailRetry : (int)EQueueStatus.Error;
                                s.ErrorMsg = res == null ? string.Empty : ((int)res.StatusCode).ToString();
                                orderInfo.Status = s.RetryTime > 3 ? (int)EOrderStatus.FailRetry : (int)EOrderStatus.ErrorEmail;
                            }

                            context.Entry(s).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            context.SaveChanges();

                            context.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "HandleEmail exception order: " + s.Id.ToString());
                        }

                    }
                }
            }
        }

        public async Task HandlerProduct()
        {
            try
            {
                var items = _queueRepository.Find(s => s.Type == (int)EQueueType.Product && (s.Status == (int)EQueueStatus.Create || s.Status == (int)EQueueStatus.Error)).ToList();

                var batchSize = 100;

                for (int i = 0; i < items.Count; i += batchSize)
                {
                    var batchItem = items.GetRange(i, Math.Min(batchSize, items.Count - i));
                    var taskLst = new List<Task>();
                    batchItem.ForEach(s =>
                    {
                        taskLst.Add(ProcessTask(s));
                    });
                    await Task.WhenAll(taskLst);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandlerProduct exception: ");
            }

            async Task ProcessTask(Queue s)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                    {
                        try
                        {
                            context.Database.BeginTransaction();

                            s.Status = (int)EQueueStatus.Processing;
                            context.Entry(s).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            context.SaveChanges();

                            var orderInfo = context.Order.FirstOrDefault(x => x.Id == s.OrderId);

                            var res = await _client.PostAsJsonAsync<string>("External/product", s.Id.ToString());

                            if (res != null && res.IsSuccessStatusCode)
                            {
                                s.Status = (int)EQueueStatus.Complete;
                                orderInfo.Status = (int)EOrderStatus.SentProduct;
                            }
                            else
                            {
                                s.RetryTime = s.RetryTime == null ? 1 : s.RetryTime++;
                                s.Status = s.RetryTime > 3 ? (int)EQueueStatus.FailRetry : (int)EQueueStatus.Error;
                                s.ErrorMsg = res == null ? string.Empty : ((int)res.StatusCode).ToString();
                                orderInfo.Status = s.RetryTime > 3 ? (int)EOrderStatus.FailRetry : (int)EOrderStatus.ErrorProduct;
                            }

                            context.Entry(s).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            context.SaveChanges();

                            context.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "HandlerProduct exception order: " + s.Id.ToString());
                        }

                    }
                }

            }
        }
    }
}
