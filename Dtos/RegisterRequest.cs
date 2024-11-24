using JwtCookiesScheme.Types;

namespace JwtCookiesScheme.Dtos
{
    public class RegisterRequest
    {
        public string UserEmail { set; get; }=string.Empty;
        public string UserPassword { set; get; } = string.Empty;
        public string ConfirmPassword { set; get; } = string.Empty;
        public string UserName {  set; get; } = string.Empty;
        public string UserPhone {  set; get; } = string.Empty;
        public string UserFirstName {  set; get; } = string.Empty; 
        public string UserLastName { set; get; }=string.Empty;
        public string UserRole { set; get; } = RoleEnum.GUEST.ToString();
    }
}
