using BusStation.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using BusStation.Middlewares;
using Microsoft.OpenApi.Models;
using System.Reflection;
using NSwag;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<StationContext>(options =>
{
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=BusDrivingDB;Trusted_Connection=True;");
});
//StationContext context = new StationContext(new DbContextOptions<StationContext>());
builder.Services.AddOpenApiDocument();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    app.UseSwaggerUI();

    app.UseOpenApi();
    app.UseSwaggerUi3();

    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.UseRequestKeyChecker();

app.Run();
