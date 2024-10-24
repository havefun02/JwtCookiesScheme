namespace JwtCookiesScheme.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public List<string> UserPermissions { set; get; } = new List<string>();
    }
}
