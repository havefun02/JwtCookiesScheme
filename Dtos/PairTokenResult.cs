namespace JwtCookiesScheme.Dtos
{
    public class PairTokenResult
    {
        public string AccessToken { set; get; }=string.Empty;
        public string RefreshToken { set; get; }=string.Empty;

    }
}
