using Auction_API.Dto;
using Auction_API.Dto.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auction_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            var userExists = await _userManager.FindByNameAsync(dto.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO
                {
                    Status = "Error",
                    Message = "User already exists!"
                });
            IdentityUser user = new()
            {
                Email = dto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = dto.UserName
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                });

            // Add the subscription type claim for the user
            await _userManager.AddClaimAsync(user, new Claim("SubscriptionType", dto.SubscriptionType));

            return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null && await _userManager.CheckPasswordAsync(existingUser, dto.Password))
            {
                List<Claim> authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, existingUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id)
            };

                var existingClaims = await _userManager.GetClaimsAsync(existingUser);
                authClaims.AddRange(existingClaims);

                var token = GetToken(authClaims);


                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized(new ResponseDTO { Status = "Error", Message = "Invalid Authentication" });
        }

        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
