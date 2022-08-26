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
                .MinimumLength(8);
        }
    }
}