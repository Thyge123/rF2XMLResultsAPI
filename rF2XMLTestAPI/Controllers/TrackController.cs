using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;
using rF2XMLTestAPI.Model;
using System.Collections.Generic;

namespace rF2XMLTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : Controller
    {
        private TracksManager _manager;
        //private DBDriversManager _manager = new DBDriversManager();

        public TrackController(RaceResultContext context, Context context2)
        {
            //DB
            _manager = new TracksManager(context, context2);

            // Non DB
            //_manager = new UsersManager();
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<RaceResults>> Get()
        {
            IEnumerable<RaceResults> list = _manager.GetAllTracks();
            if (list == null || list.Count() == 0)
            {
                return NoContent();
            }
            else
            {
                return Ok(list);
            }
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("laps/{trackVenue}")]
        public IActionResult GetLapsAtTrack(string trackVenue)
        {
            var laps = _manager.GetLapsAtTrack(trackVenue);
            if (laps == null || laps.Count() == 0)
            {
                return NotFound();
            }
            else {                
                return Ok(laps);
            }
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("AddTrack")]  
        public ActionResult<RaceResults> Addtrack(string TrackCourse)
        {
            try
            {
                _manager.AddTrack(TrackCourse);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("UpdateTrack")]
        public ActionResult<RaceResults> UpdateTrack(string TrackCourse)
        {
            try
            {
                _manager.UpdateTrack(TrackCourse);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("DeleteTrack")]
        public ActionResult<RaceResults> DeleteTrack(int id)
        {
            try
            {
                _manager.DeleteTrack(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
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

