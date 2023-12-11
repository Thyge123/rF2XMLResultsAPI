using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;
using rF2XMLTestAPI.Model;
using System.Text.Json;

namespace rF2XMLTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class rF2XMLController : Controller
    {
        private rFactorXMLManager _manager;
        //private rFactorXMLManager _manager = new rFactorXMLManager();

        public rF2XMLController(DriverContext driverContext, LapsContext lapsContext, RaceResultContext raceResultContext)
        {
            //DB
            _manager = new rFactorXMLManager(driverContext, lapsContext, raceResultContext);

            // Non DB
            //_manager = new rFactorXMLManager();
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetRaceResults")]
        public ActionResult<Root> GetRaceResults()
        {
            try
            {
                var result = _manager.LoadLatestResult();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetAllXMLResults")]
        public ActionResult<List<FileContent>> GetAllXMLFilesInDirectoryWithContents(string directoryPath)
        {
            try
            {
                var result = _manager.GetAllXmlFilesInDirectoryWithContents(directoryPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

    }
}
