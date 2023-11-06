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
        /// <summary>
        /// Gets all buses
        /// </summary>
        /// <param name="requestKey"></param>
        /// <returns>List of buses</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/buses   
        ///     
        /// </remarks>
        /// <response code="200">Returns bus object with writen "id"</response>
        /// <response code="404">If did not find any bus</response>
        /// <response code="403">Request is bad or you did not send a required header</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("api/buses")]        
        public IResult OnGetBuses([FromHeader(Name = "RequestKey")] string requestKey)
        {
            Bus[]? buses = _stationContext.Buses.IgnoreAutoIncludes().ToArray();

            if (buses.Length == 0 || buses is null) return Results.NotFound();

            return Results.Json(buses);
        }
        /// <summary>
        /// Gets bus with some "id"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestKey"></param>
        /// <returns>One object of buses with some "id"</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/buses/{id}
        ///     
        /// </remarks>
        /// <response code="200">Returns bus object with writen "id"</response>
        /// <response code="404">If did not find any bus</response>
        /// <response code="403">Request is bad or you did not send a required header</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("api/bus/{id}")]
        [Produces("application/json")]
        public IResult OnGetBus(int id, [FromHeader(Name = "RequestKey")] string requestKey)
        {
            Bus? bus = _stationContext.Buses.IgnoreAutoIncludes().Where(b => b.Id == id).FirstOrDefault();

            if (bus is null) return Results.NotFound();

            return Results.Json(bus);
        }
        /// <summary>
        /// Change bus with some "id"
        /// </summary>
        /// <param name="busInfo"></param>
        /// <param name="requestKey"></param>
        /// <returns>Changed bus</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/buses/{id}
        ///     {
        ///         "id": 1,
        ///         "number": 24,
        ///         "capacity": 15
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns changed bus object with writen "id"</response>
        /// <response code="404">If did not find any bus</response>
        /// <response code="403">Request is bad or you did not send a required header</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut("api/buses")]
        public async Task<IResult> OnPutBus([FromBody]BusInfo busInfo, [FromHeader(Name = "RequestKey")] string requestKey)
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
