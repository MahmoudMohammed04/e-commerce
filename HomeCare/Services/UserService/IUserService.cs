using HomeCare.Services.Result;

namespace HomeCare.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResult<UserDataResponse>> GetUserData(string UserId);
        Task<ServiceResult<bool>> ChangePassword(string userId, string oldPassword, string newPassword);
        Task<ServiceResult<bool>> ChangePhoneNumber(string userId, string phoneNumber);
        Task<ServiceResult<bool>> ChangeEmail(string userId, string email);

        Task<ServiceResult<bool>> ChangeUsername(string userId, string username);

    }

    public class UserDataResponse
    {
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
