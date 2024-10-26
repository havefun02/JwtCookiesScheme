
using CRUDFramework;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtCookiesScheme.Services
{
    public class LockoutService : ILockoutService
    {
        private readonly IRepository<Lockout,DatabaseContext> _context;
        private const int _maxFailedAttempts = 5;
        private const int _lockoutDurationMinutes = 1;

        public LockoutService(IRepository<Lockout, DatabaseContext> context)
        {
            _context = context;
        }

        public async Task<bool> IsLockedOutAsync(string ssid)
        {
            var user = await _context.GetDbSet().SingleOrDefaultAsync(t=>t.SsId==ssid);
            if (user == null)
            {
                await _context.GetDbSet().AddAsync(new Entities.Lockout { SsId = ssid,FailedLoginAttempts=1 });
                await _context.GetDbContext().SaveChangesAsync();
                return false;
            }
            if (user.LockoutEndTime.HasValue && user.LockoutEndTime.Value > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }

        public async Task RegisterFailedAttemptAsync(string ssid)
        {
            var user = await _context.GetDbSet().SingleOrDefaultAsync(t => t.SsId == ssid);
            if (user == null) throw new ArgumentException("User not found.");

            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= _maxFailedAttempts)
            {
                user.LockoutEndTime = DateTime.UtcNow.AddMinutes(_lockoutDurationMinutes);
                user.FailedLoginAttempts = 0; 
            }

            await _context.GetDbContext().SaveChangesAsync();


        }

        public async Task ResetFailedAttemptsAsync(string ssid)
        {
            var user = await _context.GetDbSet().SingleOrDefaultAsync(t => t.SsId == ssid);
            if (user == null) throw new ArgumentException("User not found.");

            user.FailedLoginAttempts = 0;
            user.LockoutEndTime = null;
            await _context.GetDbContext().SaveChangesAsync();


        }
    }
}
