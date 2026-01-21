using FluentValidation;
using Shared.IdentityModule;

namespace CraftoriaApp.Validators
{
    public sealed class RegisterationDtoValidators : AbstractValidator<RegisterDto>
    {
        public RegisterationDtoValidators()
        {

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .Length(3, 50).WithMessage("First name must be between 3 and 50 characters")
                .Matches(@"^[A-Za-z]+$").WithMessage("First name must contain only letters");


            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last Name is required.")
                .Length(3, 50).WithMessage("Last name must be between 3 and 50 characters")
                .Matches(@"^[A-Za-z]+$").WithMessage("Last name must contain only letters");


            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$")
                .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");

            RuleFor(RuleFor => RuleFor.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .IsInEnum().WithMessage("Invalid role type.");

            RuleFor(x => x.YearsOfExperience)
                 .Must((dto, exp) => dto.Role == RoleType.Expert || exp == null)
                 .WithMessage("ExperienceYear must be null for all roles except Expert");

            RuleFor(x => x.Portfolio)
                 .Must((dto, exp) => dto.Role == RoleType.Expert || exp == null)
                 .WithMessage("Portfolio must be null for all roles except Expert");

            

        }
    }

}
