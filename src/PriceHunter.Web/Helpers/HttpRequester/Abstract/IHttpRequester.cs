using PriceHunter.Web.Data.Base;
using static PriceHunter.Web.Helpers.HttpRequester.Concrete.HttpRequester;

namespace PriceHunter.Web.Helpers.HttpRequester.Abstract
{
    public interface IHttpRequester
    {
        Task<RequesterResponse<DataResponse<ResponseT>>> GetAsync<ResponseT>(string url, bool includeToken);
        Task<RequesterResponse<DataResponse<ResponseT>>> PostAsync<RequestT, ResponseT>(string url, RequestT requestModel, bool includeToken);
        Task<RequesterResponse<DataResponse<ResponseT>>> PutAsync<RequestT, ResponseT>(string url, RequestT requestModel, bool includeToken);
    }
}