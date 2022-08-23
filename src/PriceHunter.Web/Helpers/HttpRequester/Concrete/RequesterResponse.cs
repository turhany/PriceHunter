namespace PriceHunter.Web.Helpers.HttpRequester.Concrete
{
    public partial class HttpRequester
    {
        public class RequesterResponse<T>
        {
            public bool IsSuccess { get; set; }
            public T Response { get; set; }
        }
    }
}