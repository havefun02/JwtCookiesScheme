namespace JwtCookiesScheme.ViewModels
{
    public class UserItemViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<string> UserRole { get; set; } = new List<string>();
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string FullName()
        {
            return UserFirstName + UserLastName;
        }

        public string RoleString()
        {
            return string.Join(", ", UserRole);
        }

    }
}
