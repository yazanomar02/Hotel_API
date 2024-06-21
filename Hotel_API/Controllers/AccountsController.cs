using Domain.Entites;
using Domain.ViewModels;
using Hotel_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AccountsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }



        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate([FromBody] AuthRequest request)
        {
            if (
                configuration["User:UserName"] == request.UserName
                && configuration["User:Password"] == request.Password
                )
            {
                //              <-------- Generate Token -------->

                var claim = new List<Claim>();

                claim.Add(new Claim("given_name", "yazan"));
                claim.Add(new Claim("family_name", "omar"));
                claim.Add(new Claim("course", "Midad-11"));
                claim.Add(new Claim("sub", "123"));
                claim.Add(new Claim(ClaimTypes.Role, configuration["User:Role"]));



                var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:SecretKey"])); // إعداد المفتاح

                var signingCredantial = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256); // HmacSha256: خوارزمية التشفير

                // إعداد خصائص ال Token
                var securityToken = new JwtSecurityToken
                    (configuration["Authentication:Issuer"],
                    configuration["Authentication:Audience"],
                    claim,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(10),
                    signingCredantial
                    );

                // Object --> String        (Token)
                var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

                return Ok(token);
            }

            return Unauthorized();
        }
    }
}
