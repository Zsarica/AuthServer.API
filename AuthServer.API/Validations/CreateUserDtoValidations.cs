using AuthServer.Core.Dtos;
using FluentValidation;

namespace AuthServer.API.Validations
{
    public class CreateUserDtoValidations:AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidations()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is requiredddd").EmailAddress().WithMessage("Email is wrong");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password requiredddd");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName requiredddd");
        }
    }
}
