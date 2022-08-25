using FluentValidation;
using HelpersToolbox.Extensions;
using PriceHunter.Contract.Service.Product;
using PriceHunter.Resources.Service;

namespace PriceHunter.Business.Product.Validator
{
    public class ProductSupplierInfoMappingServiceModelValidator : AbstractValidator<ProductSupplierInfoMappingServiceModel>
    {
        public ProductSupplierInfoMappingServiceModelValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(ProductSupplierInfoMappingServiceModel.Url)))
                .When(p => !p.Url.IsValidUrl()).WithMessage(ServiceResponseMessage.INVALID_INPUT_ERROR)
                .MaximumLength(1000).WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, nameof(ProductSupplierInfoMappingServiceModel.Url), 1000));
        }
    }
}
