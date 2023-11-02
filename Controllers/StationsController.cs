using BusStation.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BusStation.Controllers
{
    public class StationsController : Controller
    {
        private readonly StationContext _stationContext;
        public StationsController(StationContext stationContext)
        {
            _stationContext = stationContext;
        }
        [HttpPost("api/{route}/stations")]        
        public async Task<IResult> OnPostStation([FromBody]StationInfo stationBody, [FromHeader]string requestKey, string route)
        {
            Infrastructure.Models.Route? routeDB = _stationContext.Routes.Where(r => r.Name == route).FirstOrDefault();
            if (routeDB == null) return Results.NotFound();

            var json = String.Empty;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }
            //StationInfo stationInfo = JsonSerializer.Deserialize<StationInfo>(stationBody)!;
            StationInfo stationInfo = stationBody;

            Station? leftStation = _stationContext.Stations.Where(s => s.Name == stationInfo.leftStationName).FirstOrDefault();
            if (leftStation == null) return Results.NotFound();

            Station newStation = new Station(stationInfo.id, stationInfo.name, leftStation.Id, stationInfo.lengthToLeftStation, 0, 0);
            newStation.Routes.Add(routeDB);

            leftStation.LengthToRight = newStation.LengthToLeft;
            leftStation.RightStationId = newStation.Id;

            routeDB.StationsOnRoute.Add(newStation);

            _stationContext.SaveChanges();

            return Results.Json(newStation);
        }

        public record StationInfo(int id, string name, string leftStationName, int lengthToLeftStation); 
    }
}
