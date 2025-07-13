using ApiService.Handler.Products;
using ApiService.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shared.AutoMapper;
using Shared.EF;
using Shared.Repositories;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(s =>
{
    s.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IV_OrderDetailRepository, V_OrderDetailRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ExternalHost"));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<ICheckOutService, CheckOutService>();

builder.Services.AddAutoMapper((s) =>
{
    s.AddProfile<MappingProfile>();
});

var productGenerator = new ProductGenerate();
builder.Services.AddSingleton(productGenerator);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
