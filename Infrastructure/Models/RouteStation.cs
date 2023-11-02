namespace BusStation.Infrastructure.Models
{
    public class RouteStation
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int StationId { get; set; }
        public RouteStation() {}
        public RouteStation(int id, int routeId, int stationId)
        {
            Id = id;
            RouteId = routeId;
            StationId = stationId;
        }
        
    }
}
