using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly IManagersService _managersService;

        public ManagersController(IManagersService managersService)
        {
            _managersService = managersService;
        }

        [HttpPost("addManager")]
        public IActionResult addManager([FromBody] ManagersDTO manager)
        {
            switch (_managersService.addManager(manager))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("not valid manager");
                case 3:
                    return Conflict("the email exists in the system");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpPut("updateManager")]
        public IActionResult updateManager([FromBody] ManagersDTO manager)
        {
            //validation here

            switch (_managersService.updateManager(manager))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("admin not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpDelete("deleteManager/{email}")]
        public IActionResult deleteAdmin([FromRoute] string email)
        {
            switch (_managersService.deleteManager(email))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return BadRequest("there are sites that cannot be deleted, so the manager is not deleteable");
                case 3:
                    return NotFound("manager not found");
                case 4:
                    return BadRequest("there is no manager email");
                default:
                    return BadRequest("something went wrong");
            }
        }

        //[HttpGet("getManager")]
        //public IActionResult getManager([FromRoute] string email)
        [HttpGet("getManager/{email}")]
        public IActionResult getManager(string email)
        {
            ManagersDTO manager = _managersService.getManager(email);
            if(manager == null)
            {
                return BadRequest("something went wrong");
            }
            return Ok(manager);
        }
    }
}
