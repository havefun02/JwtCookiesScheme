namespace JwtCookiesScheme.Interfaces
{
    public interface ILockoutService
    {
        Task<bool> IsLockedOutAsync(string ssid);
        Task RegisterFailedAttemptAsync(string ssid);
        Task ResetFailedAttemptsAsync(string ssid);
    }
}
