using FluentValidation;

namespace JwtCookiesScheme.Dtos.Validation
{
    public class ChangePasswordValidation : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidation() { }
    }
}