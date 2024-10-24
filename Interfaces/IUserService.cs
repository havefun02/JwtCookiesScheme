using JwtCookiesScheme.Entities;

namespace JwtCookiesScheme.Interfaces
{
    public interface IUserService<TUser> where TUser : User
    {
        Task<IEnumerable<TUser>> GetAllUser();
        Task EditProfile(TUser user);
        Task DeleteProfile(string UserId);
        Task<TUser> GetProfile(string UserId);

    }
}
