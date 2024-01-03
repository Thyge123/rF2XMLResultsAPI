using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;
using rF2XMLTestAPI.Model;

namespace rF2XMLTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : Controller
    {
        private DriversManager _manager;

        public DriverController(DriverContext driverContext)
        {
            _manager = new DriversManager(driverContext);
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetDrivers")]
        public ActionResult<Driver> GetDrivers()
        {
            try
            {
                var result = _manager.GetDrivers();
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
        [HttpGet("GetDriver")]
        public ActionResult<Driver> GetDriver(int id)
        {
            try
            {
                var result = _manager.GetDriver(id);
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
        [HttpPost("AddDriver")]
        public ActionResult<Driver> AddDriver(string firstName, string lastName)
        {
            try
            {
                _manager.AddDriver(firstName, lastName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("UpdateDriver")]
        public ActionResult<Driver> UpdateDriver(string firstName, string lastName)
        {
            try
            {
                _manager.UpdateDriver(firstName, lastName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("DeleteDriver")]
        public ActionResult<Driver> DeleteDriver(int id)
        {
            try
            {
                _manager.DeleteDriver(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
