using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RollCallSystem.Database;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RollCallSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTTokensController : ControllerBase
    {
        public IConfiguration _configuration;
        public readonly ApplicationDbContext _context;

        public JWTTokensController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(LoginUser loginUser)
        {
            if (loginUser != null && loginUser.Email != null && loginUser.Password != null)
            {
                User user = new User()
                {
                    Email = loginUser.Email,
                    Password = loginUser.Password,
                };

                var userData = await GetUser(user.Email, user.Password);
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
                if (user != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Email", user.Email),
                        new Claim("Password", user.Password)
                    };

                    //Get all roles from the database (i.e. Teacher, Student)
                    var roles = await _context.Roles.ToListAsync();
                    if(roles == null) return NoContent();

                    try
                    {
                        //Get the first role from my role list which I got above, that matches the roleId of the user that has logged in
                        claims.Add(new Claim(ClaimTypes.Role, roles.FirstOrDefault(x => x.Id == userData.RoleId).Name));
                    }
                    catch
                    {
                        return BadRequest("Invalid Credentials");
                    }
                    

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.key));
                    var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        jwt.Issuer,
                        jwt.Audience,
                        claims.ToArray(),
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signin
                    );
                    return Ok((new JwtSecurityTokenHandler().WriteToken(token)));
                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }

            else
                {
                    return BadRequest("Invalid Credentials");
                }
            }


            [HttpGet]
            public async Task<User> GetUser(string userEmail, string userPassword)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail && u.Password == userPassword);
            }
        }
    }
