using EvacuationAPI;
using EvacuationAPI.AppDbContext;
using EvacuationAPI.Caching;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Debug()
//     .WriteTo.Console()
//     .CreateLogger();

// Add services to the container.

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Information) 
        .WriteTo.Console()
        .CreateLogger());
});
    

builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "EvacuationAPI";
    }

);

builder.Services.AddControllers();


builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IEvacuationPlanService, EvacuationPlanService>();
builder.Services.AddScoped<IEvacuationService, EvacuationService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestingDB"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();