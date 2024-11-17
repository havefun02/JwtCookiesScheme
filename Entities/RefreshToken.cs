namespace JwtCookiesScheme.Entities
{
    public class RefreshToken
    {
        public string Id { set; get; }
        public string Value { set; get; }
        public DateTime TokenExpiredAt { get; set; }
        public bool isInvoked { get; set; }
        public string? UserId { set; get; }
        public User? User { get; set; }
    }
}
