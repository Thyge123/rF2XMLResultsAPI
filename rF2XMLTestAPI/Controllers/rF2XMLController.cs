using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;
using rF2XMLTestAPI.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace rF2XMLTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class rF2XMLController : Controller
    {
        private rFactorXMLManager _manager;
        private JsonSerializerOptions _jsonSerializerOptions;
        //private rFactorXMLManager _manager = new rFactorXMLManager();

        public rF2XMLController(DriverContext driverContext, LapsContext lapsContext, RaceResultContext raceResultContext)
        {
            //DB
            _manager = new rFactorXMLManager(driverContext, lapsContext, raceResultContext);

            // Non DB
            //_manager = new rFactorXMLManager();
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
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
        [HttpGet("GetCustomFile")]
        public ActionResult<Root> GetCustomFile(string path)
        {
            try
            {
                var result = _manager.GetCustomFile(path);
                var serializedResult = JsonSerializer.Serialize(result, _jsonSerializerOptions);
                return Ok(serializedResult);
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
