using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using travels_server_side.Entities;
using travels_server_side.Iservices;
using travels_server_side.Models;


namespace travels_server_side.Controllers

{

    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IValidatorService _validatorService;

        public UsersController(IUsersService usersService, IValidatorService validatorService)
        {
            this._usersService = usersService;
            this._validatorService = validatorService;
        }

        [HttpPost("addUser")]
        public IActionResult addUser([FromBody] UsersDTO user)
        {
            string err = _validatorService.userValidation(user);
            if (err != null)//checks whether all required fields are correct
            {
                return BadRequest($"form error! " + err);
            }
            if (!_usersService.AvailableEmail(user.email))//check if the email is available
            {
                //return BadRequest($"This email is already registered in the system");
                return Conflict($"This email is already registered in the system");
            }
            switch (_usersService.addUser(user))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return BadRequest("not valid user");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpGet("getusers")]
        public IActionResult getUsers()
        {
            List<UsersDTO> usersList = _usersService.getUsers();
            if(usersList == null)
            {
                return NotFound();
            }
            return Ok(usersList);
        }

        [HttpPut("updateUser")]
        public IActionResult updateUser([FromBody] UsersDTO user)
        {
            //autorization here

            switch (_usersService.updateUser(user))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("user not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpDelete("deleteuser/{useremail}")]
        //public IActionResult deleteAdmin([FromBody] int siteId)
        public IActionResult deleteUser([FromRoute] string userEmail)
        {
            switch (_usersService.deleteUser(userEmail))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("user not found");
                default:
                    return BadRequest("something went wrong");
            }
        }

        [HttpGet("getuserbyemail/{userEmail}")]
        public IActionResult getUserByEmail(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("error! empty email");
            }
            UsersDTO user = _usersService.getUserByEmail(userEmail);
            return Ok(user);
        }

        [HttpGet("getUserSitesRatings/{userEmail}")]
        public IActionResult getUserSitesRatings([FromRoute] string userEmail)
        {
            //get user from token

            List<SiteRatingsDTO> ratings = _usersService.getUserRatings(userEmail);
            return Ok(ratings);
        }

        [HttpPost("addSitesRating")]
        public IActionResult addSitesRating([FromBody] SiteRatingsDTO rating)
        {
            switch (_usersService.addUserRating(rating))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("not valid rating");
                default:
                    return BadRequest("something went wrong");
            }
        }

        /*
        [HttpPut("updateSiteRating")]
        public IActionResult updateSiteRating([FromBody] SiteRatingsDTO rating)
        {
            switch (_usersService.updateRating(rating))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("not valid rating");
                default:
                    return BadRequest("something went wrong");
            }
        }
        */

        [HttpGet("getUserPreferences/{userEmail}")]
        public IActionResult getUserPreferences([FromRoute] string userEmail)//temp
        {
            //get user from token

            List<UserPreferencesDTO> userPref = _usersService.getUserPref(userEmail);
            return Ok(userPref);
        }

        /*
        [HttpGet("getPreferences")]
        public IActionResult getPreferences()
        {
            List<UserPreferencesDTO> preferences = _usersService.getPreferences();
            return Ok(preferences);
        }
        */

        [HttpPost("addPreference")]
        public IActionResult addPreference([FromBody] UserPreferencesDTO preference)
        {
            switch (_usersService.addUserPref(preference))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("not valid preference");
                default:
                    return BadRequest("something went wrong");
            }
        }

        /*
        [HttpPut("updatePreference")]
        public IActionResult updateReference([FromBody] UserPreferencesDTO preference)
        {
            switch (_usersService.updateUserPref(preference))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("user preference not found");
                default:
                    return BadRequest("something went wrong");
            }
        }
        */

        //[HttpDelete("deletePreference")]
        [HttpDelete("deletePreference/{userEmail}/{categoryId}")]
        //public IActionResult deletePreference([FromBody] UserPreferencesDTO preference)
        public IActionResult deletePreference([FromRoute] string userEmail, int categoryId)
        {
            UserPreferencesDTO preference = new UserPreferencesDTO()
            {
                userEmail = userEmail,
                categoryId = categoryId
            };
            switch (_usersService.deletePreference(preference))
            {
                case 0:
                    return BadRequest("changes not saved");
                case 1:
                    return Ok();
                case 2:
                    return NotFound("user preference not found");
                default:
                    return BadRequest("something went wrong");
            }
        }
    }


}

