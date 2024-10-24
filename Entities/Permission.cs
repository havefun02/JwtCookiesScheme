namespace JwtCookiesScheme.Entities
{
    public class Permission
    {
        public required string PermissionId { set; get; }
        public required string PermissionName { set; get; }
        public virtual ICollection<RolePermissions>? RolePermissions { set; get; }


    }
}
