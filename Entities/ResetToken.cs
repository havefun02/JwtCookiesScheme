namespace JwtCookiesScheme.Entities
{
    public class ResetToken
    {
        public required string TokenId { get; set; }
        public required DateTime TokenExpiredAt { get; set; }
        public required string TokenSerect { get; set; }
        public required string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }

    }
}
