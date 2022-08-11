﻿using FluentValidation;
using PriceHunter.Business.UserProduct.Validator;
using PriceHunter.Contract.Service.Product;
using PriceHunter.Resources.Service; 

namespace PriceHunter.Business.Product.Validator
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequestServiceRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, nameof(CreateProductRequestServiceRequest.Name)))
                .MaximumLength(200)
                .WithMessage(string.Format(ServiceResponseMessage.PROPERTY_MAX_LENGTH_ERROR, nameof(CreateProductRequestServiceRequest.Name), 200));

            RuleForEach(x => x.UrlSupplierMapping).SetValidator(new ProductSupplierInfoMappingServiceModelValidator());

        }
    }
}
