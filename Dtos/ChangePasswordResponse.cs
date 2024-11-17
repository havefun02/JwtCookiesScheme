using JwtCookiesScheme.Types;

namespace JwtCookiesScheme.Dtos
{
    public class ChangePasswordResponse
    {
        public Result ChangePasswordResult { get; set; }
        public string ChangePasswordSuccess { get; set; }

        public string ChangePasswordErrorMessage { get; set; } = string.Empty;

    }
}
