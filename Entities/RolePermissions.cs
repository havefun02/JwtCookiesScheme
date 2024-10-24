namespace JwtCookiesScheme.Entities
{
    public class RolePermissions
    {
        public required string RoleId { get; set; }
        public required string PermissionId { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}
