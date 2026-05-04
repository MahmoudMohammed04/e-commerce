using HomeCare.Extentions;
using HomeCare.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController( IUserService userService)
        {
            _userService = userService;
        }
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<UserDataResponse>> GetUser()
        {
            var user = await _userService.GetUserData(GetUserId());
            
            return user.Success ?  Ok(user.Data) : user.ErrorToGenericActionResult();
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromQuery] string oldPassword, [FromQuery] string newPassword)
        {
            var result = await _userService.ChangePassword(GetUserId(), oldPassword, newPassword);
            return result.Success ? Ok(result.Data) : result.ErrorToActionResult();
        }

        [HttpPatch("UpdateEmail")]
        public async Task<IActionResult> UpdateEmail([FromQuery] string newEmail)
        {
            var result = await _userService.ChangeEmail(GetUserId(), newEmail);
            return result.Success ? Ok(result.Data) : result.ErrorToActionResult();
        }

        [HttpPatch("UpdatePhone")]
        public async Task<IActionResult> UpdatePhone([FromQuery] string newPhone)
        {
            var result = await _userService.ChangePhoneNumber(GetUserId(), newPhone);
            return result.Success ? Ok(result.Data) : result.ErrorToActionResult();
        }

        [HttpPatch("UpdateUsername")]
        public async Task<IActionResult> UpdateUsername([FromQuery] string newUsername)
        {
            var result = await _userService.ChangeUsername(GetUserId(), newUsername);
            return result.Success ? Ok(result.Data) : result.ErrorToActionResult();
        }
       
    }
}
