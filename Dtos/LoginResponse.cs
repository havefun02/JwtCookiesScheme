using JwtCookiesScheme.Types;
namespace JwtCookiesScheme.Dtos
{
    public class LoginResponse
    {
        public Result LoginResult { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string LoginErrorMessage { get; set; } = string.Empty;


    }
}
