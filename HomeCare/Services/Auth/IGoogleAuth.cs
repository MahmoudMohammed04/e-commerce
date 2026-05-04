using HomeCare.Models.UserSchema;
using HomeCare.Services.Result;

namespace HomeCare.Services.Auth
{
    public interface IGoogleAuth
    {
        public Task<ServiceResult<User>> GoogleLoginRequest(string idToken);
    }
}
