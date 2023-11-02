using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusStation.Infrastructure.Models
{
    public class Station
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Route> Routes { get; set; } = new List<Route>();
        public int? LeftStationId { get; set; }
        public int LengthToLeft { get; set; }
        public int? RightStationId { get; set; }
        public int LengthToRight { get; set; }
        public Station() { }
        public Station(int id, string name)
            :this()
        {
            Id = id;
            Name = name;
        }
        public Station(int id, string name, int leftStationId, int lengthToLeft)  
            :this(id, name)
        {
            LeftStationId = leftStationId;
            LengthToLeft = lengthToLeft;
        }
        public Station(int id, string name, int leftStationId, int lengthToLeft, int rightStationId, int lengthToRight)
            :this(id, name, leftStationId, lengthToLeft)
        {
            RightStationId = rightStationId;
            LengthToRight = lengthToRight;
        }
    }
}
