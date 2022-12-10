using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Controllers
{
    [Produces("application/json")]
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class VisitsController : ControllerBase
    {
        private readonly IValidatorService _validatorService;
        private readonly IVisitsService _visitsService;

        public VisitsController(IValidatorService validatorService, IVisitsService visitsService)
        {
            this._validatorService = validatorService;
            this._visitsService = visitsService;
        }

        [HttpPost("addVisit_v1")]
        public IActionResult addVisit_v1([FromBody] VisitsDTO visit)
        {
            switch (_visitsService.addVisit(visit))
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

        [HttpPost("addVisit")]
        public IActionResult addVisit([FromBody] VisitsDTO visit)
        {
            int visitId = _visitsService.addVisit(visit);
            if(visitId <= 0)
            {
                return BadRequest("Your changes could not be saved");
            }
            return Ok(visitId);
        }

        [HttpPut("updateVisit")]
        public IActionResult updateVisit([FromBody] VisitsDTO visit)
        {
            //autorization here

            switch (_visitsService.updateVisit(visit))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("visit not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpPut("updateVisitDatetime")]
        public IActionResult updateVisitDatetime([FromBody] VisitsDTO visit)
        {
            switch (_visitsService.updateVisitDatetime(visit))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("visit not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpPut("updateRating")]
        //public IActionResult updateRating([FromBody] VisitsDTO visit)
        public IActionResult updateRating([FromBody] VisitedSite site)
        {
            //temp hardcoded, the user email need to be retrieved from the token
            //string userEmail = "string2";
            
            //switch (_visitsService.updateRating(keys.userEmail, keys.siteId, keys.))
            switch (_visitsService.updateRating(site.userEmail, site.siteId, site.rating))
            {
                case 0:
                    return BadRequest("changes not saved");
                case >0:
                    return Ok();
                case -1:
                    return NotFound("visit not found");
                default:
                    return BadRequest("something went wrong");
            }
        }
        

        [HttpDelete("deleteVisit")]
        public IActionResult deleteVisit([FromBody] VisitKeys keys)
        {
            switch (_visitsService.deleteVisit(keys))
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

        [HttpDelete("deleteVisitById/{visitId}")]
        public IActionResult deleteVisitById([FromRoute] int visitId)
        {
            switch (_visitsService.deleteVisitById(visitId))
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

        [HttpGet("getVisit")]
        public IActionResult getVisit([FromBody] VisitKeys keys)
        {
            VisitsDTO visit = _visitsService.getVisit(keys);
            return Ok(visit);
        }

        [HttpGet("getVisitById")]
        public IActionResult getVisitById([FromRoute] int visitId)
        {
            VisitsDTO visit = _visitsService.getVisitsById(visitId);
            return Ok(visit);
        }

        [HttpGet("getVisits")]
        public IActionResult getVisits()
        {
            List<VisitsDTO> visits = _visitsService.getVisits();
            return Ok(visits);
        }

        [HttpGet("getTravelVisits/{travelId}")]
        public IActionResult getTravelVisits([FromRoute] int travelId)
        {
            List<VisitsDTO> visits = _visitsService.getTravelVisits(travelId);
            return Ok(visits);
        }

        /*
        [HttpPut("updateRating")]
        public IActionResult updateRating([FromBody] VisitsDTO visit)
        {
            if (!this._validatorService.visitValidator(visit))
            {
                return BadRequest($"fill all needed properties");
            }
            //get userEmail from token
            string emailFromToken = getUserEmailFromToken();
            if(string.IsNullOrEmpty(emailFromToken))
            {
                return Unauthorized($"You are not logged in");
            }
            if(this._visitsService.updateVisit(new VisitsDTO {
                userEmail = emailFromToken, 
                siteId = visit.siteId, 
                rating = visit.rating}))
            {
                return Ok();
            }
            return BadRequest($"change not saved");
        }
        */
        /*
        [HttpGet("demoGetVisitedSites")]
        public IActionResult demoGetVisitedSites()
        {
            
            string userEmail = getUserEmailFromToken();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized($"You are not logged in");
            }
            
            List<object> visitedSites = this._visitsService.DemoGetVisitedSites(userEmail);
            if (visitedSites == null)
            {
                return BadRequest();
            }
            return Ok(visitedSites);
        }
        */

        [HttpGet("getVisitedSitesByEmail/{userEmail}")]
        public IActionResult getVisitedSitesByEmail([FromRoute] string userEmail)
        {
            List<VisitedSite> visitedSites = _visitsService.getUserVisitedSites(userEmail);

            if (visitedSites == null)
            {
                return BadRequest();
            }
            return Ok(visitedSites);
        }

        [HttpGet("getUserVisitedSites")]
        public IActionResult getUserVisitedSites()
        {
            string userEmail = getUserEmailFromToken();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized($"You are not logged in");
            }
            //real
            List<VisitedSite> visitedSites = _visitsService.getUserVisitedSites(userEmail);

            //test
            //List<object> visitedSites = _visitsService.DemoGetVisitedSites(userEmail);
            if (visitedSites == null)
            {
                return BadRequest();
            }
            return Ok(visitedSites);
        }


        private string getUserEmailFromToken()
        {
            var authheader = Response.Headers.Values;//.FirstOrDefault();
            //getting user email from token
            //var userEmail = User.Claims.FirstOrDefault(x => x.Type.Equals("userEmail", StringComparison.InvariantCultureIgnoreCase));
            return authheader.ToString(); 
        }
    }
}
