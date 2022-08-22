namespace PriceHunter.Web.Data.Base
{
    public class DataResponse<T> : BaseResponse
    {
        public DataResponse()
        {
            Data = default;
        }

        public T Data { get; set; }
    }
}
