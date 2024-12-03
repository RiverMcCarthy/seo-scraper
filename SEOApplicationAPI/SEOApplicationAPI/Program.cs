using SEOApplicationAPI.Interfaces;
using SEOApplicationAPI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SEOApplicationAPI.DataAccessLayer.Contexts;
using SEOApplicationAPI.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<SEODbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add configuration
var baseUrl = builder.Configuration["AppSettings:ApiBaseUrl"];
var numberOfResults = builder.Configuration["AppSettings:NumberOfResults"];

// Add Serilog Logging
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/SEOApplicationAPI.log", rollingInterval: RollingInterval.Day));

// Add services to the container.

builder.Services.AddControllers();

// Register HttpClient and ScrapingService
builder.Services.AddHttpClient<ScrapingService>();
builder.Services.AddScoped<IScrapingService, ScrapingService>();
builder.Services.AddScoped<IRankingService, RankingService>();

builder.Services.AddAutoMapper(typeof(RankingMappingProfile));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowVueApp");

app.MapControllers();

app.Run();
