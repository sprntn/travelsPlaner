using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Controllers
{
    //[Authorize]//יש להפעיל רק אחרי שהכל עובד

    [Produces("application/json")]
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly ISitesService _sitesService;
        private readonly IValidatorService _validatorService;

        public SitesController(ISitesService sitesService, IValidatorService validatorService)
        {
            this._sitesService = sitesService;
            this._validatorService = validatorService;
        }

        [HttpPost("addSite")]
        public IActionResult addSite([FromBody] SitesDTO site)
        {
            switch (_sitesService.addSite(site))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return BadRequest("not valid site");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpGet("getSite/{siteId}")]
        public IActionResult getSite([FromRoute]int siteId)
        {
            SitesDTO site = _sitesService.getSite(siteId);
            return Ok(site);
        }

        [HttpGet("getSites")]
        public IActionResult getSites()
        {
            List<SitesDTO> sites = _sitesService.getSites();
            return Ok(sites);
        }

        [HttpGet("getTopSites")]
        public IActionResult getTopSites()
        {
            List<SitesDTO> sites = _sitesService.getSuggestedSites();
            return Ok(sites);
        }

        [HttpGet("getUserSites/{userEmail}")]//test, the email need to retrieved from token
        //[HttpGet("getUserSites")]//test, the email need to retrieved from token
        public IActionResult getUserEmail([FromRoute]string userEmail)//test, the email need to retrieved from token
        //public IActionResult getUserEmail()
        {
            //autorization here

            List<SitesDTO> userSites = _sitesService.getSuggestedSites(userEmail);
            //List<SitesDTO> userSites = _sitesService.getSites();
            return Ok(userSites);
        }

        [HttpGet("getManagerSites/{managerEmail}")]//test, the email need to retrieved from token
        public IActionResult getManagrSites(string managerEmail)
        {
            //autorization here

            List<SitesDTO> managerSites = _sitesService.getManagerSites(managerEmail);
            return Ok(managerSites);
        }

        [HttpPut("updateSite")]
        public IActionResult updateSite([FromBody] SitesDTO site)
        {
            //autorization here

            switch (_sitesService.updateSite(site))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("site not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpDelete("deleteSite/{siteId}")]
        //public IActionResult deleteAdmin([FromBody] int siteId)
        public IActionResult deleteAdmin([FromRoute] int siteId)
        {
            switch (_sitesService.deleteSite(siteId))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("site not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpGet("getCategories")]
        public IActionResult getCategories()
        {
            List<SiteCategoriesDTO> categories = this._sitesService.getCategories();
            return Ok(categories);
        }

        /*
        //[AllowAnonymous]
        [HttpGet("demoGetAllSites/")]
        public IActionResult demoGetAllSites()
        {
            List<SitesDTO> sites = this._sitesService.DemoGetSites();
            return Ok(sites);
        }

        [HttpGet("getUserSites")]
        public IActionResult getUserSites()
        {
            var authheader = Response.Headers.Values;//.FirstOrDefault();
            //getting user email from token
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.Equals("userEmail", StringComparison.InvariantCultureIgnoreCase));
            if(userEmail == null)
            {
                return Unauthorized();
            }
            //List<SitesDTO> sites = this._sitesService.GetSites(userEmail.Value);//real
            //return Ok(sites);

            //temp
            return Ok(userEmail.Value);
        }
        */

        /*
        [HttpGet("demoGeSites/{siteId}")]
        public IActionResult demoGetSite(int siteId)
        {
            SitesDTO site = this._sitesService.DemoGetSiteById(siteId);
            return Ok(site);
        }
        */

        /*
        [HttpGet("demoGetUserSites/{userEmail?}")]
        public IActionResult demoGeUsertSites(string userEmail = null)
        {
            List<SitesDTO> sites = this._sitesService.DemoGetSites();
            return Ok(sites);
        }
        */
    }
}
