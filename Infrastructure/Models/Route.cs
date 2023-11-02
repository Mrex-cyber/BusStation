using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

namespace BusStation.Infrastructure.Models
{
    public class Route
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Kilometers {
            get {
                int kilometers = 0;
                foreach (Station station in StationsOnRoute) {
                    kilometers += station.LengthToRight;
                }
                return kilometers;
            }
        }
        public List<Station> StationsOnRoute { get; private set; } = new List<Station>();
        public int StationsCount { get => StationsOnRoute.Count; }
        [ForeignKey("bus_id")]
        public int? BusId { get; set; }
        public Bus? Bus { get; set; }
        public Route() { }
        public Route(int id, string name)
            : this()
        {
            Id = id;
            Name = name;
        } 
        public Route(int id, string name, int busId)
            :this(id, name)
        {
            BusId = busId;
        }        
        public void AddStation(Station station)
        {
            StationsOnRoute.Add(station);
        }
    }
}
