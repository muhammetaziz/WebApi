using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.DTO;
using ProductsAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(UserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                DateAdded = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }


        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.Select(i => UserDTO(i)).ToListAsync();
            if (_userManager == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(users);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserWithId(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) { return BadRequest(new { message = "Email Hatası." }); }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (result.Succeeded) { return Ok(new { token = GenerateJWT(user) }); }
            return Unauthorized();
        }
        private static UserDTO UserDTO(AppUser u)
        {
            return new UserDTO { Email = u.Email, FullName = u.FullName, UserName = u.UserName };
        }
        private object GenerateJWT(AppUser user)
        {
            var tokenHendler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? "");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName ?? ""),

                    }),
                Expires=DateTime.UtcNow.AddDays(1),
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),
                Issuer="muhammetaziz.com"
            };
            var token=tokenHendler.CreateToken(tokenDescriptor);
            return tokenHendler.WriteToken(token);
        }
    }
}
