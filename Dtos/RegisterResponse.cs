using JwtCookiesScheme.Types;

namespace JwtCookiesScheme.Dtos
{
    public class RegisterResponse
    {
        public Result RegisterResult { get; set; }
        public string? SuccessMessage { get; set; } =string.Empty;
        public string? ErrorMessage { get; set; }=string.Empty;
    }
}
