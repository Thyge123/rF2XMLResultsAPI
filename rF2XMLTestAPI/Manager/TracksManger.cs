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
        private LapsContext _lapsContext;
        private DriverContext _driversContext;
        private Context _context;
        public TracksManger(RaceResultContext raceResultContext, LapsContext lapsContext, DriverContext driverContext, Context context)
        {  
            _raceResultContext = raceResultContext;
            _lapsContext = lapsContext;
            _driversContext = driverContext;
            _context = context; 
        }

        public IEnumerable<RaceResults> GetAllTracks(string sortBy = null)
        {
            IEnumerable<RaceResults> Tracks = from TrackVenue in _raceResultContext.RaceResults
                                              where (sortBy == null)
                                              select TrackVenue;
            return Tracks;
        }

        public IEnumerable<object> GetLapsAtTrack(string trackVenue)
        {
            
            
            var bestLapsQuery = from lap in _context.Laps
                                join raceResult in _context.RaceResults on lap.RaceResultsId equals raceResult.Id
                                where raceResult.TrackVenue == trackVenue
                                group lap by lap.DriverId into driverLaps
                                select new
                                {
                                    DriverId = driverLaps.Key,
                                    BestLapTime = driverLaps.Min(l => l.LapTime)
                                };

            var query = from driver in _context.Drivers
                        join bestLap in bestLapsQuery on driver.Id equals bestLap.DriverId
                        select new
                        {
                            DriverName = driver.Name,
                            BestLapTime = bestLap.BestLapTime,
                            TrackVenue = trackVenue
                        };
           
            return query.ToList();
            
        }

 
    }
}
