using PriceHunter.Common.Attributes; 
using System.Reflection; 

namespace PriceHunter.Common.Extensions
{
    public static class EnumExtensions
    {
        public static Guid GetDatabaseId(this Enum enumValue)
        {
            var databaseId = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DatabaseIdAttribute>()?
                .Id ?? string.Empty;

            return string.IsNullOrWhiteSpace(databaseId) ? Guid.Empty : Guid.Parse(databaseId);
        }
    }
}
