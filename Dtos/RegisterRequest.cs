namespace JwtCookiesScheme.Dtos
{
    public class RegisterRequest
    {
        public string UserEmail { set; get; }=string.Empty;
        public string UserPassword { set; get; } = string.Empty;
        public string ConfirmPassword { set; get; } = string.Empty;
        public string UserName {  set; get; } = string.Empty;
        public string UserPhone {  set; get; } = string.Empty;
    }
}
