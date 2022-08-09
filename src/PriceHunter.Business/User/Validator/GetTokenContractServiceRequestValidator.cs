using FluentValidation;
using PriceHunter.Contract.Service.User;
using PriceHunter.Resources.Service;

namespace PriceHunter.Business.User.Validator
{
    public class GetTokenContractServiceRequestValidator: AbstractValidator<GetTokenContractServiceRequest>
    {
        public GetTokenContractServiceRequestValidator()
        {
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(GetTokenContractServiceRequest.Email)))
                .MaximumLength(320).WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, nameof(GetTokenContractServiceRequest.Email), 320))
                .EmailAddress().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_INVALID, nameof(GetTokenContractServiceRequest.Email)));

            RuleFor(request => request.Password)
                .NotEmpty()
                .WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(GetTokenContractServiceRequest.Password)))
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("'Password' must contain one or more capital letters.")
                .Matches("[a-z]").WithMessage("'Password' must contain one or more lowercase letters.")
                .Matches(@"\d").WithMessage("'Password' must contain one or more digits.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("'Password' must contain one or more special characters.");
        }
    }
}