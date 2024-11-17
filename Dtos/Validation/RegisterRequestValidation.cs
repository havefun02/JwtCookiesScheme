using FluentValidation;

namespace JwtCookiesScheme.Dtos.Validation
{
    public class RegisterRequestValidation:AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidation() {
            RuleFor(x => x.UserEmail)
               .NotEmpty().WithMessage("Email is required")
               .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.UserPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.UserPassword).WithMessage("Passwords do not match");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required");

            RuleFor(x => x.UserPhone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number");

        }
    }
}
