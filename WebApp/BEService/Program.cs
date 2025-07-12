using BEService;
using BEService.Services;
using BEService.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shared.EF;
using Shared.Repositories;

var builder = Host.CreateApplicationBuilder(args);

//builder.Services.AddHostedService<EmailWorker>();
builder.Services.AddHostedService<ProductWorker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IHandlerService, HandlerService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();

builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ExternalHost"));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var host = builder.Build();
host.Run();
