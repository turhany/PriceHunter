using FluentValidation;
using FluentValidation.Validators;
using PriceHunter.Contract.Service.User;
using PriceHunter.Resources.Service;
 
namespace PriceHunter.Business.User.Validator
{
    public class UpdateUserRequestValidator: AbstractValidator<UpdateUserRequestServiceRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(request => request.FirstName)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, "First Name"))
                .MaximumLength(40).WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, "First Name", 40));

            RuleFor(request => request.LastName)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, "Last Name"))
                .MaximumLength(40).WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, "Last Name", 40));

            RuleFor(request => request.Email)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, "Email"))
                .MaximumLength(320).WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, "Email", 320))
                .EmailAddress(EmailValidationMode.Net4xRegex).WithMessage(string.Format(ServiceResponseMessage.PROPERTY_INVALID, "Email"));

            RuleFor(request => request.Password)                
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("'Password' must contain one or more capital letters.")
                .Matches("[a-z]").WithMessage("'Password' must contain one or more lowercase letters.")
                .Matches(@"\d").WithMessage("'Password' must contain one or more digits.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("'Password' must contain one or more special characters.")
                .When(p => !string.IsNullOrWhiteSpace(p.Password));
        }
    }
}