using FluentValidation;
using PriceHunter.Contract.Service.UserProduct;
using PriceHunter.Resources.Service;

namespace PriceHunter.Business.UserProduct.Validator
{
    public class UpdateUserProductRequestValidator : AbstractValidator<UpdateUserProductRequestServiceRequest>
    {
        public UpdateUserProductRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(UpdateUserProductRequestServiceRequest.Name)))
                .MaximumLength(200)
                .WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, nameof(UpdateUserProductRequestServiceRequest.Name), 200));

            RuleForEach(x => x.UrlSupplierMapping).SetValidator(new UrlSupplierMappingServiceModelValidator());
        }
    }
}
