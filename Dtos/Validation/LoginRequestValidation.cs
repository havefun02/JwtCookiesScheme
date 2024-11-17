using FluentValidation;
namespace JwtCookiesScheme.Dtos.Validation
{
    public class LoginRequestValidation:AbstractValidator<LoginRequest>
    {
        public LoginRequestValidation() {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password at least long 6 characters");
        }
    }
}
