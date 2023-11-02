using BusStation.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BusStation.Controllers
{
    public class TimeController : Controller
    {
        private readonly StationContext _stationContext;
        public TimeController(StationContext stationContext)
        {
            _stationContext = stationContext;
        }
        [HttpGet("api/time/{busNumber}/{hours}/{minutes}")]
        public IResult OnGetBusByTime(string busNumber, int hours, int minutes)
        {
            int allMinutes = (hours - 8) * 60 + minutes; 

            Bus? bus = _stationContext.Buses.Where(b => b.Number == busNumber).FirstOrDefault();
            Infrastructure.Models.Route? route = _stationContext.Routes.Include(r => r.StationsOnRoute).Where(r => r.BusId == bus.Id).FirstOrDefault();
            if (route == null || bus == null) return Results.NotFound();


            double minutesForOneWay = route.Kilometers / bus.Speed * 60; 
            bool fromLeftToRight = Math.Floor(allMinutes / minutesForOneWay) % 2 == 0; 
            int minutesFromStartPoint = Convert.ToInt32(Math.Round(allMinutes % minutesForOneWay)); 

            List<Station> stations = route.StationsOnRoute;
            string stationName = "";
            int minToStation = 0;
            int allLength = 0;
            if (fromLeftToRight)
            {
                for (int i = 0; i < stations.Count; i++)
                {
                    if (allLength / (double)bus.Speed * 60 > minutesFromStartPoint)
                    {
                        stationName = stations[i].Name;
                        minToStation = (int)Math.Round(allLength / (double)bus.Speed * 60 - minutesFromStartPoint);
                        break;
                    }
                    allLength += stations[i].LengthToRight;
                }
            }
            else
            {
                for (int i = stations.Count - 1; i >= 0; i--)
                {
                    if ((int)Math.Round(allLength / (double)bus.Speed * 60) > minutesFromStartPoint)
                    {
                        stationName = stations[i].Name;
                        minToStation = (int)Math.Round(allLength / (double)bus.Speed * 60 - minutesFromStartPoint);
                        break;
                    }
                    allLength += stations[i].LengthToLeft;
                }
            }

            if (minToStation > 60)
            {
                if (minToStation / 60 == 1) return Results.Json($"Bus({busNumber}) is {minToStation / 60} hour and {minToStation % 60} minutes from the station \"{stationName}\" station");
                else return Results.Json($"Bus({busNumber}) is {minToStation / 60} hours and {minToStation % 60} minutes from the station \"{stationName}\" station");
            }
            return Results.Json($"Bus({busNumber}) is {minToStation} minutes from the station \"{stationName}\" station");
        }
        [HttpPost("api/time/{routeName}/people/{hours}/{minutes}")]
        public async Task<IResult> CheckPeopleCount(string routeName, int hours, int minutes)
        {
            Infrastructure.Models.Route? route = _stationContext.Routes.Include(r => r.Bus).Include(r => r.StationsOnRoute).Where(r => r.Name == routeName).FirstOrDefault();

            if (route is null) return Results.NotFound();

            var json = String.Empty;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }
            PeopleInfo stationsInfo = JsonSerializer.Deserialize<PeopleInfo>(json)!;
            var entered = stationsInfo.entered;
            var exited = stationsInfo.exited;



            int allMinutes = (hours - 8) * 60 + minutes;

            double minutesForOneWay = route.Kilometers / route.Bus.Speed * 60; // 180 min
            bool fromLeftToRight = Math.Floor(allMinutes / minutesForOneWay) % 2 == 0; // 2.55 % 2 == 0
            int minutesFromStartPoint = Convert.ToInt32(Math.Round(allMinutes % minutesForOneWay));

            List<Station> stations = route.StationsOnRoute;

            int result = 0;

            int fullWay = (int)Math.Floor(allMinutes / minutesForOneWay);
            double allWay = allMinutes / minutesForOneWay;
            int currentInBus = 0;

            int wayToRight = WayToRight();
            int wayToLeft = WayToLeft();

            bool isRight = true;
            result += fullWay / 2 * (wayToLeft + wayToRight);
            if (fullWay % 2 != 0)
            {
                result += wayToRight;
            }

            int allPeopleOnLastWay = 0;
            int allLength = 0;
            if ((double)fullWay != allWay)
            {
                if (fromLeftToRight)
                {
                    for (int i = 0; i < stations.Count; i++)
                    {
                        if (allLength / (double)route.Bus.Speed * 60 > minutesFromStartPoint)
                        {
                            break;
                        }
                        else
                        {
                            int enteredNumber = int.Parse(entered[i]);
                            int exitedNumber = int.Parse(exited[i]);

                            currentInBus -= exitedNumber;
                            if (currentInBus < 0) currentInBus = 0;
                            if (currentInBus + enteredNumber > 20)
                            {
                                allPeopleOnLastWay += 20 - currentInBus;
                                currentInBus = 20;
                            }
                            else
                            {
                                allPeopleOnLastWay += enteredNumber;
                                currentInBus += enteredNumber;
                            }
                        }
                        allLength += stations[i].LengthToRight;
                    }
                }
                else
                {
                    allLength = stations[stations.Count - 1].LengthToLeft;
                    for (int i = stations.Count - 2; i > 0; i--)
                    {
                        if ((int)Math.Round(allLength / (double)route.Bus.Speed * 60) > minutesFromStartPoint)
                        {
                            break;
                        }
                        else
                        {
                            int enteredNumber = int.Parse(entered[i]);
                            int exitedNumber = int.Parse(exited[i]);

                            currentInBus -= exitedNumber;
                            if (currentInBus < 0) currentInBus = 0;
                            if (currentInBus + enteredNumber > 20)
                            {
                                allPeopleOnLastWay += 20 - currentInBus;
                                currentInBus = 20;
                            }
                            else
                            {
                                allPeopleOnLastWay += enteredNumber;
                                currentInBus += enteredNumber;
                            }
                        }
                        allLength += stations[i].LengthToLeft;
                    }
                }
            }


            int WayToRight()
            {
                int allPeople = 0;
                for (int i = 0; i < entered.Length; i++)
                {
                    int enteredNumber = int.Parse(entered[i]);
                    int exitedNumber = int.Parse(exited[i]);

                    currentInBus -= exitedNumber;
                    if (currentInBus < 0) currentInBus = 0;
                    if (currentInBus + enteredNumber > 20)
                    {
                        allPeople += 20 - currentInBus;
                        currentInBus = 20;
                    }
                    else
                    {
                        allPeople += enteredNumber;
                        currentInBus += enteredNumber;
                    }
                }

                return allPeople;
            }
            int WayToLeft()
            {
                int allPeople = 0;
                for (int i = entered.Length - 2; i > 0; i--)
                {
                    int enteredNumber = int.Parse(entered[i]);
                    int exitedNumber = int.Parse(exited[i]);

                    currentInBus -= exitedNumber;
                    if (currentInBus < 0) currentInBus = 0;
                    if (currentInBus + enteredNumber > 20)
                    {
                        allPeople += 20 - currentInBus;
                        currentInBus = 20;
                    }
                    else
                    {
                        allPeople += enteredNumber;
                        currentInBus += enteredNumber;
                    }
                }

                return allPeople;
            }

            result += allPeopleOnLastWay;
            return Results.Json($"Route({routeName}) have {result} people till this time!");
        }
        
    }
}
