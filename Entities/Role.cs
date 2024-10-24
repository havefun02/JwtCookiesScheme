namespace JwtCookiesScheme.Entities
{
    public class Role
    {
        public required string RoleId { get; set; }
        public required string RoleName { get; set; }
        public virtual IEnumerable<User>? Users { get; set; }
        public virtual IEnumerable<RolePermissions>? RolePermissions { get; set; }

    }
}
