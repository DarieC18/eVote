using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Response;
using Newtonsoft.Json;

namespace EVote360.Web.Helpers
{
    public class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetUserSession(UserResponseDto user)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("User", JsonConvert.SerializeObject(user));
        }

        public void ClearUserSession()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("User");
        }

        public bool HasUser()
        {
            var user = _httpContextAccessor.HttpContext?.Session.GetString("User");
            return !string.IsNullOrEmpty(user);
        }

        public UserResponseDto? GetUserSession()
        {
            var user = _httpContextAccessor.HttpContext?.Session.GetString("User");
            return string.IsNullOrEmpty(user) ? null : JsonConvert.DeserializeObject<UserResponseDto>(user);
        }
    }
}
