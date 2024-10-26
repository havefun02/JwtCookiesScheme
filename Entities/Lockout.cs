namespace JwtCookiesScheme.Entities
{
    public class Lockout
    {
        public int Id { get; set; }
        public string? SsId { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEndTime { get; set; }
    }
}
