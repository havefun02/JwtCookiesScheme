using Microsoft.AspNetCore.Identity;

namespace JwtCookiesScheme.Entities
{
    public class Role:IdentityRole
    {
        public virtual ICollection<UserRole>? UserRoles { get; set; }

    }
}
