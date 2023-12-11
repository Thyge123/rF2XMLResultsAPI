using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;
using rF2XMLTestAPI.Model;

namespace rF2XMLTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : Controller
    {
        private TracksManger _manager;
        //private DBDriversManager _manager = new DBDriversManager();

        public TrackController(RaceResultContext context, Context context2)
        {
            //DB
            _manager = new TracksManger(context, context2);

            // Non DB
            //_manager = new UsersManager();
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<RaceResults>> Get([FromQuery] string? sort_by)
        {
            IEnumerable<RaceResults> list = _manager.GetAllTracks(sort_by);
            if (list == null || list.Count() == 0)
            {
                return NoContent();
            }
            else
            {
                return Ok(list);
            }
        }

        [HttpGet("laps/{trackVenue}")]
        public IActionResult GetLapsAtTrack(string trackVenue)
        {
            var laps = _manager.GetLapsAtTrack(trackVenue);
            return Ok(laps);
        }
    }
    /*
    // GET: api/Track/BestLaps/{trackVenue}
    [HttpGet("BestLaps/{trackVenue}")]
    public ActionResult<Dictionary<string, Lap>> GetBestLaps(string trackVenue)
    {
        var bestLaps = _manager.GetBestLapsForAllDriversAtTrack(trackVenue);
        if (bestLaps.Count == 0)
        {
            return NotFound();
        }

        return Ok(bestLaps);
    }
    */
}

