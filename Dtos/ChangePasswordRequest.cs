namespace JwtCookiesScheme.Dtos
{
    public class ChangePasswordRequest
    {
        public string UserName { set; get; } = string.Empty;
        public string Password { set; get; } = string.Empty;
        public string NewPassword { set; get; } = string.Empty;
        public string ConfirmPassword { set; get; } = string.Empty;


    }
}
