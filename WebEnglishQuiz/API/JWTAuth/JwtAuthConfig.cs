using API.Models;
using static API.Common.Variables;
using System.Security.Claims;
using API.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace API.JWTAuth
{
    public static class JwtAuthConfig
    {
        public static JwtResponse CreateToken(IConfiguration _configuration, User user)
        {
            string Role = user.Role == false ? "Admin" : "NormalUser";

			List<Claim> claims = new List<Claim> { 
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtResponse
            {
                Token = jwt,
                Username = user.Username,
                Fullname = user.FirstName + " " + user.LastName,
                Role = Role
			};
        }
    }
}
