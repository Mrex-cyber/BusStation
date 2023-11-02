using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusStation.Infrastructure.Models
{
    public class Bus
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public int Capacity { get; set; }
        public int Speed { get; set; }
        [ForeignKey("route_id")]
        public int? RouteId { get; set; }
        public Route? Route { get; set; }
        public Bus() { }
        public Bus(int id, string number, int capacity, int speed)
            : this()
        {
            Id = id;
            Number = number;
            Capacity = capacity;
            Speed = speed;
        }
        public Bus(int id, string number, int capacity, int speed, int routeId)
            :this(id, number, capacity, speed)
        {
            Id = id;
            Number = number;
            Capacity = capacity;
            Speed = speed;
            RouteId = routeId;
        }
    }
}
