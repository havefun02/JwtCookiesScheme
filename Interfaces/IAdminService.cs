using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Entities;

namespace JwtCookiesScheme.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<User>> GetDashBoard(string adminId);
        Task UpdateUser(string adminId, EditRequest user);
        Task<User> GetUserData(string adminId, string userId);
        Task DeleteDataUser(string adminId, string userId);
    }
}
