using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Controllers
{
    //[Authorize]//יש להפעיל רק אחרי שהכל עובד

    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("getAdmins")]
        public IActionResult getAdmins()
        {
            List<AdminDTO> admins = _adminService.getAdmins();
            return Ok(admins);
        }

        [HttpGet("getAdmin/{adminEmail}")]//test. need to be retrieved from token
        public IActionResult getAdmin(string adminEmail)
        {
            AdminDTO admin = _adminService.getAdmin(adminEmail);
            if(admin == null)
            {
                return BadRequest("admin not found");
            }
            return Ok(admin);
        }

        [HttpPost("addAdmin")]
        public IActionResult addAdmin([FromBody] AdminDTO admin)
        {
            //validation here

            switch (_adminService.addAdmin(admin))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return BadRequest("not valid admin");
                case 3:
                    return Conflict("the email exists in the system");
                default: 
                    return BadRequest("something went wrong");
            }
        }

        [HttpPut("updateAdmin")]
        public IActionResult updateAdmin([FromBody] AdminDTO admin)
        {
            //validation here

            switch (_adminService.updateAdmin(admin))
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

        [HttpDelete("deleteAdmin")]
        public IActionResult deleteAdmin([FromBody] string email)
        {
            switch (_adminService.deleteAdmin(email))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("admin not found");
                case 3:
                    return BadRequest("there is no admin email");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpPost("addSiteCategory")]
        public IActionResult addSiteCategory([FromBody] SiteCategoriesDTO category)
        {
            //validation here

            switch (_adminService.addSiteCategory(category))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return BadRequest("not valid category");
                case 3:
                    return Conflict("this category already exists in the system");
                default:
                    return BadRequest("something went wrong");
            }
        }

        //get site categories in sites controller

        [HttpPut("updateSiteCategory")]
        public IActionResult updateSiteCategory([FromBody] SiteCategoriesDTO category)
        {
            //validation here
            switch (_adminService.updateSiteCategory(category))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("category not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpDelete("deleteSiteCategory/{categoryId}")]
        public IActionResult deleteSiteCategory([FromRoute] int categoryId)
        {
            switch (_adminService.deleteSiteCategory(categoryId))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("category not found");
                case 3:
                    return BadRequest("category in use cannot be deleted");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpGet("getManagers")]
        public IActionResult getManagers()
        {
            List<ManagersDTO> managers = _adminService.getManagers();
            if(managers == null)
            {
                return BadRequest("something went wrong");
            }
            return Ok(managers);
        }

    }
}
