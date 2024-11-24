using JwtCookiesScheme.Types;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace JwtCookiesScheme.Dtos
{
    public class EditRequest
    {
        public string UserEmail { set; get; } = string.Empty;
        public string UserPhone { set; get; } = string.Empty;
        public string UserFirstName { set; get; } = string.Empty;
        public string UserLastName { set; get; } = string.Empty;
        public List<string> UserRoles { set; get; } = new List<string>() ;
        public List<SelectListItem> RoleList { set; get; } = new List<SelectListItem>
            {
                new SelectListItem { Text = "GUEST", Value = "GUEST" },
                new SelectListItem { Text = "ADMIN", Value = "ADMIN" },
                new SelectListItem { Text = "OWNER", Value = "OWNER" }
            };
    }
}
