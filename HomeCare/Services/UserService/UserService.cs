using HomeCare.Context;
using HomeCare.Models.UserSchema;
using HomeCare.Services.Result;
using Microsoft.AspNetCore.Identity;

namespace HomeCare.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        public UserService( AppDbContext context , UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ServiceResult<UserDataResponse>> GetUserData(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            var data = new UserDataResponse()
            {
                Username = user.UserName,
                Phone = user.PhoneNumber,
                Email = user.Email,
            };

            return new ServiceResult<UserDataResponse>(data);
        }

        public async Task<ServiceResult<bool>> ChangeUsername(string userId, string username)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new ServiceResult<bool>("User not found", ErrorTypeEnum.NOT_FOUND);

            var result = await _userManager.SetUserNameAsync(user, username);
            return new ServiceResult<bool>(result.Succeeded);
        }

        public async Task<ServiceResult<bool>> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new ServiceResult<bool>("User not found", ErrorTypeEnum.NOT_FOUND);

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return new ServiceResult<bool>(result.Succeeded);
        }

        public async Task<ServiceResult<bool>> ChangePhoneNumber(string userId, string phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new ServiceResult<bool>("User not found", ErrorTypeEnum.NOT_FOUND);

            var result = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
            return new ServiceResult<bool>(result.Succeeded);
        }

        public async Task<ServiceResult<bool>> ChangeEmail(string userId, string email)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
                return new ServiceResult<bool>("User not found" , ErrorTypeEnum.NOT_FOUND);
            var result = await _userManager.SetEmailAsync(user, email);

            if(result.Succeeded)
            {
                user.GoogleId = null;
            }

            return new ServiceResult<bool>(result.Succeeded);
        }
    }
}
