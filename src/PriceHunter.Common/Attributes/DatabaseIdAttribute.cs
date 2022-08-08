namespace PriceHunter.Common.Attributes
{
    public class DatabaseIdAttribute : Attribute
    {
        public string Id { get; set; }

        public DatabaseIdAttribute(string id)
        {
            Id = id;
        }
    }
}
