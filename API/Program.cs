using Microsoft.EntityFrameworkCore;
using Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var dbStr = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddDbContext<DayPricesDbContext>(o => o
    .UseMySql(dbStr, ServerVersion.AutoDetect(dbStr))
#if DEBUG
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
#endif
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
