using FluentValidation;
using PriceHunter.Contract.Service.User;
using PriceHunter.Resources.Service;

namespace PriceHunter.Business.User.Validator
{
    public class RefreshTokenContractServiceRequestValidator: AbstractValidator<RefreshTokenContractServiceRequest>
    {
        public RefreshTokenContractServiceRequestValidator()
        {
            RuleFor(request => request.Token)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(RefreshTokenContractServiceRequest.Token)));
        }
    }
}