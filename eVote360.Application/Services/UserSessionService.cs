using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EVote360.Application.Services
{
    public class UserSessionService : IUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public void SetUserSession(UserResponseDto user)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));

            var context = _httpContextAccessor.HttpContext
                          ?? throw new InvalidOperationException("HTTP context is not available.");

            context.Session.SetString("UserSession", JsonConvert.SerializeObject(user));
        }
        public UserResponseDto? GetUserSession()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return null;

            var userSession = context.Session.GetString("UserSession");
            return string.IsNullOrWhiteSpace(userSession)
                ? null
                : JsonConvert.DeserializeObject<UserResponseDto>(userSession);
        }

        public void ClearUserSession()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return;

            context.Session.Remove("UserSession");
        }
        public bool HasUser()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return false;

            return context.Session.TryGetValue("UserSession", out _);
        }
    }
}
