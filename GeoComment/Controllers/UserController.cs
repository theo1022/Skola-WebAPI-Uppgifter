using GeoComment.Data;
using GeoComment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    public class NewUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly GeoCommentDbContext _ctx;
        private readonly UserManager<User> _userManager;

        public UserController(GeoCommentDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpPost]
        [ApiVersion("0.2")]
        [Route("register")]
        public async Task<IActionResult> RegisterNewUser(NewUser userInput)
        {
            var user = new User()
            {
                UserName = userInput.UserName
            };

            await _userManager.CreateAsync(user, userInput.Password);

            var successfullyCreated =
                await _userManager.CheckPasswordAsync(user, userInput.Password);

            if (!successfullyCreated) return BadRequest();

            var createdUser = await _userManager.FindByNameAsync(user.UserName);

            return Created("", new { username = createdUser.UserName, id = createdUser.Id });
        }
    }
}
