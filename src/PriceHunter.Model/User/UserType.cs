using System.ComponentModel.DataAnnotations;
using PriceHunter.Resources.App;

namespace PriceHunter.Model.User
{
    public enum UserType
    {
        [Display(Name = nameof(Literals.UserType_Root), ResourceType = typeof(Literals))]
        Root = 1
    }
}
