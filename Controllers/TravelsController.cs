using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Controllers
{
    //[Authorize]//יש להפעיל רק אחרי שהכל עובד

    [Produces("application/json")]
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]

    public class TravelsController : ControllerBase
    {
        private readonly ITravelsService _travelsService;

        public TravelsController(ITravelsService travelsService)
        {
            _travelsService = travelsService;
        }

        [HttpGet("getTravel/{travelId}")]
        public IActionResult getTravel([FromRoute] int travelId)
        {
            TravelsDTO travel = _travelsService.getTravel(travelId);
            return Ok(travel);
        }

        [HttpGet("getTravels")]
        public IActionResult getTravels()
        {
            List<TravelsDTO> travels = _travelsService.getTravels();
            return Ok(travels);
        }

        [HttpGet("getUserTravels/{userEmail}")]
        public IActionResult getUserTravels([FromRoute] string userEmail)
        {
            List<TravelsDTO> travels = _travelsService.getUserTravels(userEmail);
            return Ok(travels);
        }

        [HttpGet("getTravelSites/{travelId}")]
        public IActionResult getTravelSites([FromRoute] int travelId)
        {
            List<SelectedSite> sites = _travelsService.getTravelSites(travelId);
            return Ok(sites);
        }

        [HttpPost("addEmptyTravel")]
        public IActionResult addEmptyTravel([FromBody] TravelsDTO travel)
        {
            int travelId = _travelsService.addEmptyTravel(travel);
            if (travelId <= 0)
            {
                return BadRequest(0);
            }
            return Ok(travelId);
        }

        [HttpPost("addTravel")]
        public IActionResult addTravel([FromBody] TravelsDTO travel)
        {
            switch (_travelsService.addTravel(travel))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return BadRequest("not valid travel");
                default:
                    return BadRequest("something went wrong");
            }
        }



        [HttpPut("updateTravel")]
        public IActionResult updateTravel([FromBody] TravelsDTO travel)
        {
            //autorization here

            switch (_travelsService.updateTravel(travel))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("travel not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpPut("executeTravel")]
        public IActionResult executeTravel([FromBody] int travelId)
        {
            //autorization here

            switch (_travelsService.executeTravel(travelId))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("travel not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpDelete("deleteTravel/{travelId}")]
        public IActionResult deleteTravel([FromRoute] int travelId)
        {
            //get the userEmsil from token
            
            switch (_travelsService.deleteTravel(travelId))
            {
                case > 0:
                    return Ok();
                case 0:
                    return BadRequest("changes not saved");
                case -1:
                    return NotFound("travel not found");
                case -2:
                    return Unauthorized("you are not the owner of this travel");
                default:
                    return BadRequest("something went wrong");

            }
        }
    }
}
