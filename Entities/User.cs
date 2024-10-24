namespace JwtCookiesScheme.Entities
{
    public class User : UserBase
    {
        public virtual Role? UserRole { get; set; }
        public string UserPassword { get; set; } = string.Empty;
        public required string UserEmail { get; set; }
        public virtual ResetToken? UserToken { get; set; }
        public string UserPhone { get; set; } = string.Empty;
    }
}
