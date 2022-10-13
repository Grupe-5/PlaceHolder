using Microsoft.EntityFrameworkCore;
using API.Data;
using Common;
using ScraperLib;

var builder = WebApplication.CreateBuilder(args);
/* Add services to the container.*/
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DayPricesDbContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddSingleton<IFetcher, PriceFetcher>();

var app = builder.Build();

/* Configure the HTTP request pipeline.*/
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
