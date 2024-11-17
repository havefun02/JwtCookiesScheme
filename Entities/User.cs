using Microsoft.AspNetCore.Identity;

namespace JwtCookiesScheme.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { set; get; }=string.Empty;
        public string LastName { set; get; } = string.Empty;

        public override string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        public override bool PhoneNumberConfirmed { get => base.PhoneNumberConfirmed; set => base.PhoneNumberConfirmed = value; }
    }
}
