using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusStation.Infrastructure.Models
{
    public class StationContext : DbContext
    {
        public DbSet<Bus> Buses { get; set; } = null!;
        public DbSet<Route> Routes { get; set; } = null!;
        public DbSet<Station> Stations { get; set; } = null!;
        public DbSet<RouteStation>  RouteStations { get; set; } = null!;

        public StationContext(DbContextOptions<StationContext> options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=BusDrivingDB;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Bus[] buses = {
                new Bus(1, "24", 30, 10, 1),
                new Bus(3, "18", 30, 10, 2),
                new Bus(2, "8", 15, 15, 3)
            };
            Route[] routes =
            {
                new Route(1, "Dravtsi", 1),
                new Route(2, "Epicentr", 3),
                new Route(3, "Chornovola", 2),
            };
            Station[] stationsForDravtsi =
            {
                new Station(
                    1, "A",
                    0, 0,
                    2, 10),
                new Station(
                    2, "B",
                    1, 10,
                    3, 5),
                new Station(
                    3, "C",
                    2, 5,
                    4, 15),
                new Station(
                    4, "D",
                    3, 15,
                    0, 0),
            };
            Station[] stationsForEpicentr =
            {
                new Station(
                    5, "E",
                    0, 0,
                    6, 5),
                new Station(
                    6, "F",
                    5, 5,
                    7, 3),
                new Station(
                    7, "G",
                    6, 3,
                    8, 12),
                new Station(
                    8, "H",
                    7, 12,
                    0, 0),
            };
            Station[] stationsForChornovola =
            {
                new Station(
                    9, "J", 
                    0, 0, 
                    10, 25),
                new Station(
                    10, "K", 
                    9, 25, 
                    11, 10),
                new Station(
                    11, "L", 
                    10, 10, 
                    12, 10),
                new Station(
                    12, "M", 
                    11, 10, 
                    0, 0),
            };


            modelBuilder.Entity<Route>()
                .HasOne(r => r.Bus)
                .WithOne(b => b.Route)
                .HasForeignKey<Route>(r => r.BusId);

            modelBuilder.Entity<Route>()
                .HasMany(r => r.StationsOnRoute)
                .WithMany(s => s.Routes)
                .UsingEntity<RouteStation>();

            modelBuilder.Entity<Bus>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Route>()
            .Property(r => r.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Station>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();


            List<RouteStation> routeStations = new List<RouteStation>();

            List<Station> allStations = new List<Station>();
            int index = 1;
            for (int i = 0; i < 4; i++)
            {
                routeStations.Add(new RouteStation(index, routes[0].Id, stationsForDravtsi[i].Id));
                allStations.Add(stationsForDravtsi[i]);
                index++;
            }
            for (int i = 0; i < 4; i++)
            {
                routeStations.Add(new RouteStation(index, routes[1].Id, stationsForEpicentr[i].Id));
                allStations.Add(stationsForEpicentr[i]);
                index++;
            }
            for (int i = 0; i < 4; i++)
            {
                routeStations.Add(new RouteStation(index, routes[2].Id, stationsForChornovola[i].Id));
                allStations.Add(stationsForChornovola[i]);
                index++;
            }

            modelBuilder.Entity<Route>().HasData(routes);
            modelBuilder.Entity<Bus>().HasData(buses);
            modelBuilder.Entity<RouteStation>().HasData(routeStations);
            modelBuilder.Entity<Station>().HasData(allStations);
            
        }
    }
}
