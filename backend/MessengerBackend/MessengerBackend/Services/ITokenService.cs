using MessengerBackend.Models;

namespace MessengerBackend.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
    }
}
