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
        /// <summary>
        /// Gets all routes
        /// </summary>
        /// <param name="requestKey"></param>
        /// <returns>List of routes</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/routes
        ///     
        /// </remarks>
        /// <response code="200">Returns bus object with writen "id"</response>
        /// <response code="404">If did not find any bus</response>
        /// <response code="403">Request is bad or you did not send a required header</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("api/routes")]
        public IResult OnGetRoutes([FromHeader(Name = "RequireKey")] string requestKey)
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
        /// <summary>
        /// Gets route with some "id"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestKey"></param>
        /// <returns>One object of routes with some "id"</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/routes/{id}
        ///     
        /// </remarks>
        /// <response code="200">Returns route object with writen "id"</response>
        /// <response code="404">If did not find any bus</response>
        /// <response code="403">Request is bad or you did not send a required header</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("api/route/{id}")]
        public IResult OnGetRoute(int id, [FromHeader(Name = "RequireKey")] string requestKey)
        {
            Infrastructure.Models.Route? route = _stationContext.Routes.Include(r => r.StationsOnRoute).Where(r => r.Id == id).FirstOrDefault();

            if (route is null) return Results.NotFound();
            foreach (var station in route.StationsOnRoute)
            {
                station.Routes.Clear();
            }
            if (route is null) return Results.NotFound();

            return Results.Json(route);
        }
        /// <summary>
        /// Change route with some "id"
        /// </summary>
        /// <param name="routeInfo"></param>
        /// <param name="requestKey"></param>
        /// <returns>Changed route</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/buses
        ///     {
        ///         "id": 1,
        ///         "name": Dravtsi,
        ///         "stations": [Object object...]
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns route object with writen "id"</response>
        /// <response code="404">If did not find any route</response>
        /// <response code="403">Request is bad or you did not send a required header</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut("api/route")]
        public async Task<IResult> OnPutRoute([FromBody]RouteInfo routeInfo, [FromHeader(Name = "RequireKey")] string requestKey)
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
            if (routeDB.Bus is not null) routeDB.Bus.Route = null;
            return Results.Json(routeDB);
        }
    }
    public record RouteInfo(int id, string name, int[] stations);
}
