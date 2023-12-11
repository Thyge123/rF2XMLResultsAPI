using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;


namespace rF2XMLTestAPI.Manager
{
    public class TracksManger
    {
        private RaceResultContext _raceResultContext;
        private Context _context;
        public TracksManger(RaceResultContext raceResultContext, Context context)
        {  
            _raceResultContext = raceResultContext;         
            _context = context; 
        }

        public IEnumerable<RaceResults> GetAllTracks(string sortBy = null)
        {
            IEnumerable<RaceResults> Tracks = from TrackCourse in _raceResultContext.RaceResults
                                              where (sortBy == null)
                                              select TrackCourse;
            return Tracks;
        }

        public IEnumerable<object> GetLapsAtTrack(string TrackCourse)
        {
            var lapsQuery = from lap in _context.Laps
                            join raceResult in _context.RaceResults on lap.RaceResultsId equals raceResult.Id
                            where raceResult.TrackCourse == TrackCourse
                            select new
                            {
                                DriverId = lap.DriverId,
                                LapTime = lap.LapTime,
                                Sector1 = lap.Sector1,
                                Sector2 = lap.Sector2,
                                Sector3 = lap.Sector3,
                                CarType = lap.CarType,
                                CarClass = lap.CarClass,
                                Fuel = lap.Fuel,
                                VehName = lap.VehName
                            };
            var query = from driver in _context.Drivers
                        join lap in lapsQuery on driver.Id equals lap.DriverId
                        select new
                        {
                            DriverFirstName = driver.FirstName,
                            DriverLastName = driver.LastName,
                            LapTime = lap.LapTime,
                            Sector1 = lap.Sector1,
                            Sector2 = lap.Sector2,
                            Sector3 = lap.Sector3,
                            CarType = lap.CarType,
                            CarClass = lap.CarClass,
                            Fuel = lap.Fuel,
                            VehName = lap.VehName,
                            TrackCourse = TrackCourse
                        };
            return query.ToList();
        }


        public IEnumerable<object> GetBestLapAtTrack(string TrackCourse)
        {
            var bestLapsQuery = from lap in _context.Laps
                                join raceResult in _context.RaceResults on lap.RaceResultsId equals raceResult.Id
                                where raceResult.TrackCourse == TrackCourse
                                group lap by lap.DriverId into driverLaps
                                select new
                                {
                                    DriverId = driverLaps.Key,
                                    BestLapTime = driverLaps.Min(l => l.LapTime),
                                    Sector1 = driverLaps.Min(l => l.Sector1),
                                    Sector2 = driverLaps.Min(l => l.Sector2),
                                    Sector3 = driverLaps.Min(l => l.Sector3),
                                    CarType = driverLaps.Min(l => l.CarType),
                                    CarClass = driverLaps.Min(l => l.CarClass),
                                    Fuel = driverLaps.Min(l => l.Fuel),
                                    VehName = driverLaps.Min(l => l.VehName)
                                };
            var query = from driver in _context.Drivers
                        join bestLap in bestLapsQuery on driver.Id equals bestLap.DriverId
                        select new
                        {
                            DriverFirstName = driver.FirstName,
                            DriverLastName = driver.LastName,
                            BestLapTime = bestLap.BestLapTime,
                            Sector1 = bestLap.Sector1,
                            Sector2 = bestLap.Sector2,
                            Sector3 = bestLap.Sector3,
                            CarType = bestLap.CarType,
                            CarClass = bestLap.CarClass,
                            Fuel = bestLap.Fuel,
                            VehName = bestLap.VehName,
                            TrackCourse = TrackCourse
                        };
            return query.ToList();
        }
    }
}
