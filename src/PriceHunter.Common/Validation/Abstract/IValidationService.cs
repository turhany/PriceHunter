using PriceHunter.Common.Validation.Concrete;

namespace PriceHunter.Common.Validation.Abstract
{
    public interface IValidationService
    {
        ValidationResponse Validate(Type type, dynamic request);
    }
}