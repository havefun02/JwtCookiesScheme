
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
        private readonly AppUserManager _userMananger;


        public UserService(IRepository<User, DatabaseContext> repository,AppUserManager userManager) {
            _repository = repository; 
            _userMananger = userManager;    
        }
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
                var users = await context.ToListAsync();
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
                var user = await _userMananger.FindByIdAsync(UserId);
                if (user == null)
                {
                    throw new NullReferenceException("Can not find user in database");
                }
                return user;

            }
            catch 
            {
                throw;
            }
        }

    }
}
