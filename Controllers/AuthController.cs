using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Controllers
{
    [Produces("application/json")]
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            this._authService = authService;
        }

        //[HttpPost]
        //[Route("demoLogin/{user}")]
        [HttpPost("demoLogin")]
        public IActionResult demoLogin([FromBody] UsersDTO user)
        {
            if (String.IsNullOrEmpty(user.email))
            {
                return Unauthorized();
            }
            return Ok(this._authService.demoToken(user.email));//return demo token string or error message
        }

        [HttpPost("demoManagerLogin")]
        public IActionResult demoManagerLogin([FromBody] ManagersDTO manager)
        {
            if (String.IsNullOrEmpty(manager.email))
            {
                return Unauthorized();
            }
            return Ok(this._authService.demoToken(manager.email));//return demo token string or error message
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("userLogin")]
        public IActionResult userLogin([FromBody] UsersDTO user)
        {
            //validation here
            string token = _authService.authenticate(user.email);
            return Ok(new
            {
                userEmail = user.email,
                StatusCode = 200,
                message = "logged in successfully",
                jwtToken = token,
            });
        }

        [HttpPost]
        [Route("managerLogin")]
        public IActionResult managerLogin([FromBody] ManagersDTO manager)
        {
            //validation here
            string token = _authService.authenticate(manager.email);
            return Ok(new
            {
                managerEmail = manager.email,
                StatusCode = 200,
                message = "logged in successfully",
                jwtToken = token,
            });
        }

        [HttpGet("connectionTest")]
        public IActionResult connectionTest()
        {
            return Ok("server connected!");
        }

        [HttpGet("testGetBackEmail")]
        public IActionResult testToken()
        {
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.Equals("userEmail", StringComparison.InvariantCultureIgnoreCase));
            return Ok(userEmail.Value);
        }
    }
}
