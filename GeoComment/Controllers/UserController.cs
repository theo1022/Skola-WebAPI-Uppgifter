using GeoComment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    #region User formats - DTO's

    public class NewUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ReturnFormatUser
    {
        public string id { get; set; }
        public string username { get; set; }
    }
    #endregion

    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Converts a <see cref="NewUser"/> to be stored in the database and returns a format meant as output
        /// </summary>
        /// <param name="inputBody"></param>
        /// <returns><see cref="CreatedResult"/>(<see cref="ReturnFormatUser"/>)</returns>
        [HttpPost]
        [ApiVersion("0.2")]
        [Route("register")]
        public async Task<ActionResult<ReturnFormatUser>> RegisterNewUser(NewUser inputBody)
        {
            var user = new User()
            {
                UserName = inputBody.UserName
            };

            await _userManager.CreateAsync(user, inputBody.Password);

            var successfullyCreated =
                await _userManager.CheckPasswordAsync(user, inputBody.Password);

            if (!successfullyCreated) return BadRequest();

            var createdUser = await _userManager.FindByNameAsync(user.UserName);

            return Created("", new ReturnFormatUser() { username = createdUser.UserName, id = createdUser.Id });
        }
    }
}
