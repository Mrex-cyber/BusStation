using BusStation.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BusStation.Controllers
{
    public class BusController : Controller
    {
        private readonly StationContext _stationContext;
        public BusController(StationContext stationContext)
        {
            _stationContext = stationContext;
        }
        [HttpGet("api/buses")]
        public IResult OnGetBuses()
        {
            Bus[]? buses = _stationContext.Buses.IgnoreAutoIncludes().ToArray();

            if (buses.Length == 0 || buses is null) return Results.NotFound();

            return Results.Json(buses);
        }
        [HttpGet("api/bus/{id}")]
        public IResult OnGetBus(int id)
        {
            Bus? bus = _stationContext.Buses.IgnoreAutoIncludes().Where(b => b.Id == id).FirstOrDefault();

            if (bus is null) return Results.NotFound();

            return Results.Json(bus);
        }
        [HttpPut("api/buses")]
        public async Task<IResult> OnPutBus([FromBody]BusInfo busInfo)
        {

            Bus? busDB = _stationContext.Buses.Where(b => b.Id == busInfo.id).FirstOrDefault();

            if (busDB is null) return Results.NotFound();

            busDB.Id = busInfo.id;
            busDB.Number = busInfo.number;
            busDB.Capacity = busInfo.capacity;
            await _stationContext.SaveChangesAsync();

            return Results.Json(busDB);
        }            
    }
    public record BusInfo(int id, string number, int capacity);
}
