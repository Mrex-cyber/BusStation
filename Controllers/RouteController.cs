using BusStation.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace BusStation.Controllers
{
    public class RouteController : Controller
    {
        private readonly StationContext _stationContext;
        public RouteController(StationContext stationContext)
        {
            _stationContext = stationContext;
        }
        [HttpGet("api/routes")]
        public IResult OnGetRoutes()
        {
            Infrastructure.Models.Route[]? routes = _stationContext.Routes.Include(r => r.StationsOnRoute).Include(r => r.Bus).ToArray();
            foreach (var route in routes)
            {
                foreach (var station in route.StationsOnRoute)
                {
                    station.Routes.Clear();
                }
                route.Bus!.Route = null;
            }

            return Results.Json(routes);
        }

        [HttpGet("api/route/{id}")]
        public IResult OnGetRoute(int id)
        {
            Infrastructure.Models.Route? route = _stationContext.Routes.Include(r => r.StationsOnRoute).Where(r => r.Id == id).FirstOrDefault();

            foreach (var station in route.StationsOnRoute)
            {
                station.Routes.Clear();
            }
            if (route is null) return Results.NotFound();

            return Results.Json(route);
        }
        [HttpPut("api/route")]
        public async Task<IResult> OnPutRoute([FromBody]RouteInfo routeInfo)
        {           
            Infrastructure.Models.Route? routeDB = _stationContext.Routes.Where(r => r.Id == routeInfo.id).Include(r => r.StationsOnRoute).Include(r => r.Bus).FirstOrDefault();

            if (routeDB is null) return Results.NotFound();

            routeDB.Id = routeInfo.id;
            routeDB.Name = routeInfo.name;
            routeDB.StationsOnRoute[0].LengthToRight = routeInfo.stations[0];
            for(int i = 1; i < routeDB.StationsCount - 1; i++)
            {
                routeDB.StationsOnRoute[i].LengthToLeft = routeInfo.stations[i - 1];
                routeDB.StationsOnRoute[i].LengthToRight = routeInfo.stations[i];
            }
            routeDB.StationsOnRoute[routeDB.StationsCount - 1].LengthToLeft = routeInfo.stations[routeInfo.stations.Length - 1];
            
            
            await _stationContext.SaveChangesAsync();

            foreach (var station in routeDB.StationsOnRoute)
            {
                station.Routes.Clear();
            }
            routeDB.Bus.Route = null;
            return Results.Json(routeDB);
        }
    }
    public record RouteInfo(int id, string name, int[] stations);
    public record PeopleInfo(string[] entered, string[] exited);
}
