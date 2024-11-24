using JwtCookiesScheme.Types;

namespace JwtCookiesScheme.Dtos
{
    public class EditResponse
    {
        public Result EditResult { get; set; }
        public string? SuccessMessage { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; } = string.Empty;
    }

}
