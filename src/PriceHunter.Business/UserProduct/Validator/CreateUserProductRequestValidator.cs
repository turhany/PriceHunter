using FluentValidation;
using PriceHunter.Contract.Service.UserProduct;
using PriceHunter.Resources.Service;

namespace PriceHunter.Business.UserProduct.Validator
{
    public class CreateUserProductRequestValidator : AbstractValidator<CreateUserProductRequestServiceRequest>
    {
        public CreateUserProductRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(CreateUserProductRequestServiceRequest.Name)))
                .MaximumLength(200)
                .WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, nameof(CreateUserProductRequestServiceRequest.Name), 200));

            RuleForEach(x => x.UrlSupplierMapping).SetValidator(new UrlSupplierMappingServiceModelValidator());
        }
    }
}
