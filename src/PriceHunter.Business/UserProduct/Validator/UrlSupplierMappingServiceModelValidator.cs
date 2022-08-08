using FluentValidation;
using HelpersToolbox.Extensions;
using PriceHunter.Contract.Service.UserProduct;
using PriceHunter.Resources.Service;

namespace PriceHunter.Business.UserProduct.Validator
{
    public class UrlSupplierMappingServiceModelValidator : AbstractValidator<UrlSupplierMappingServiceModel>
    {
        public UrlSupplierMappingServiceModelValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(UrlSupplierMappingServiceModel.Url)))
                .When(p => !p.Url.IsValidUrl()).WithMessage(ServiceResponseMessage.INVALID_INPUT_ERROR)
                .MaximumLength(500).WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, nameof(UrlSupplierMappingServiceModel.Url), 500));
        }
    }
}
