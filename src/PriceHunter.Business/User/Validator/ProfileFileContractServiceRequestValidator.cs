using FluentValidation; 
using PriceHunter.Common.Validation;
using PriceHunter.Contract.Service.User;
using PriceHunter.Resources.Service;

namespace PriceHunter.Business.User.Validator
{
    public class ProfileFileContractServiceRequestValidator : AbstractValidator<ProfileFileContractServiceRequest>
    {
        public ProfileFileContractServiceRequestValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, "File name"));

            RuleFor(x => x.FileData)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, "File data"));

            RuleFor(x => x)
                .NotEmpty().WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, "File data"))
                .Must(IsHaveExtension).WithMessage("Extension can't be null.")
                .Must(file => IsValidMime(file, new[] { MimeValidation.MimeTypes.Jpeg, MimeValidation.MimeTypes.Jpg, MimeValidation.MimeTypes.Png })).WithMessage("Wrong file type!");
        }

        private static bool IsValidMime(ProfileFileContractServiceRequest file, string[] fileTypes)
        {
            return MimeValidation.IsValidMime(file.FileData, file.FileName, fileTypes);
        }

        private static bool IsHaveExtension(ProfileFileContractServiceRequest file)
        {
            var extension = Path.GetExtension(file.FileName);
            return !string.IsNullOrEmpty(extension);
        }
    }
}
