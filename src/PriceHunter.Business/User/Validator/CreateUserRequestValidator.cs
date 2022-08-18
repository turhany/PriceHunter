using FluentValidation;
using FluentValidation.Validators;
using PriceHunter.Common.Validation;
using Microsoft.AspNetCore.Http;
using PriceHunter.Common.Constans;
using PriceHunter.Contract.Service.User;
using PriceHunter.Resources.Service;
  
namespace PriceHunter.Business.User.Validator
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequestServiceRequest>
    {
        public CreateUserRequestValidator()
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
                .NotEmpty()
                .WithMessage(string.Format(ServiceResponseMessage.PROPERTY_REQUIRED, "Password"))
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("'Password' must contain one or more capital letters.")
                .Matches("[a-z]").WithMessage("'Password' must contain one or more lowercase letters.")
                .Matches(@"\d").WithMessage("'Password' must contain one or more digits.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("'Password' must contain one or more special characters.");

            RuleFor(x => x.Image)                    
                .Must(IsHaveExtension).WithMessage("Extension can't be null.")
                .Must(file => IsValidMime(file, new[] { MimeValidation.MimeTypes.Jpeg, MimeValidation.MimeTypes.Jpg, MimeValidation.MimeTypes.Png })).WithMessage("Wrong file type!")
                .When(p => p.Image != null);
        }

        private static bool IsValidMime(IFormFile file, string[] fileTypes)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            return MimeValidation.IsValidMime(fileBytes, file.FileName, fileTypes);
        }

        private static bool IsHaveExtension(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            return !string.IsNullOrEmpty(extension);
        }
    }
}