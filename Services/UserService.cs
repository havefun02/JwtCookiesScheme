
using CRUDFramework;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JwtCookiesScheme.Services
{
    public class UserService : IUserService<User>
    {
        private readonly IRepository<User, DatabaseContext> _repository;
        public UserService(IRepository<User, DatabaseContext> repository) { _repository = repository; }
        public async Task DeleteProfile(string UserId)
        {
            try
            {
                var user = await _repository.FindOneById(UserId);
                if (user != null)
                {
                    await _repository.Delete(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to delete user profile", ex);
            }
        }

        public async Task EditProfile(User user)
        {
            try
            {
                var updatedResult = await _repository.Update(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to update user", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllUser()
        {
            try
            {
                var context = _repository.GetDbSet();
                var users = await context.Include(u => u.UserRole).ThenInclude(r => r!.RolePermissions!).ThenInclude(rp => rp.Permission).ToListAsync();
                return users;

            }
            catch (Exception ex)
            {
                throw new Exception("Fail to get users data", ex);
            }
        }
        public async Task<User> GetProfile(string UserId)
        {
            try
            {
                var context = _repository.GetDbSet();
                var users = await context.Include(u => u.UserRole).ThenInclude(r=>r!.RolePermissions!).ThenInclude(rp=>rp.Permission).SingleOrDefaultAsync(u => u.UserId == UserId);
                if (users == null) throw new ArgumentNullException($"Can not find {nameof(users)}");
                return users;

            }
            catch (Exception ex)
            {
                throw new Exception("Fail to get user data", ex);
            }
        }

    }
}
