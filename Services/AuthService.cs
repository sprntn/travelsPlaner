using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using travels_server_side.DBcontext;
using travels_server_side.Iservices;

namespace travels_server_side.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly TravelsDbContext _travelDbContext;

        public AuthService(TravelsDbContext travelsDbContext,  IConfiguration config)
        {
            _travelDbContext = travelsDbContext;
            this._config = config;
        }

        public bool correctCredential(String userEmail, String password) 
        {
            return _travelDbContext.users.Any(u => u.email == userEmail && u.password == password);
        }

        public string demoToken(string userEmail)
        {
            if (String.IsNullOrEmpty(userEmail))
            {
                return "you are not allowed";
            }
            return "DemoToken" + userEmail;
        }

        public string authenticate(string userEmai)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            SigningCredentials credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            Claim[] claims = new Claim[]
            {
                new Claim("userEmail", userEmai)
                //new Claim(ClaimTypes.Email, userEmai)
                //new Claim(ClaimTypes.Role, "user"),//להוסיף אחר כך
            };
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
                );

            return tokenHandler.WriteToken(token); ;
        }
    }
}
