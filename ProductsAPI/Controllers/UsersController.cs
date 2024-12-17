using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.DTO;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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

        private static UserDTO UserDTO(AppUser u)
        {
            return new UserDTO { Email = u.Email, FullName = u.FullName, UserName = u.UserName };
        }
    }
}
