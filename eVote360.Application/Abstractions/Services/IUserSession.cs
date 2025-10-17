using EVote360.Application.DTOs.Response;

namespace EVote360.Application.Abstractions.Services
{
    public interface IUserSession
    {
        void SetUserSession(UserResponseDto user);
        void ClearUserSession();
        bool HasUser();
        UserResponseDto? GetUserSession();
    }
}
