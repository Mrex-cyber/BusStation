using BusStation.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using BusStation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<StationContext>(options =>
{
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=BusDrivingDB;Trusted_Connection=True;");
});
StationContext context = new StationContext(new DbContextOptions<StationContext>());
//builder.Services.AddSingleton(typeof(StationContext));
builder.Services.AddControllers();
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

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseRequestKeyChecker();

app.UseAuthorization();

app.MapControllers();

app.Run();
