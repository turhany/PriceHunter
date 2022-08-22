using PriceHunter.Web.Data.Base;
using PriceHunter.Web.Helpers.Constants; 
using System.Text;
using Blazored.Toast.Services;  
using System.Net.Http.Json; 

namespace PriceHunter.Web.Helpers
{
    public class HttpRequester
    {
        public HttpClient _httpClient { get; }
        public IToastService _toastService { get; }

        public HttpRequester(
            HttpClient httpClient,
            IToastService toastService
            )
        {
            _httpClient = httpClient;
            _toastService = toastService;
        }

        public async Task<RequesterResponse<DataResponse<ResponseT>>> GetAsync<ResponseT>(string url)
        {
            var result = await _httpClient.GetAsync(url);

            var responseModel = await result.Content.ReadFromJsonAsync<DataResponse<ResponseT>>();

            if (responseModel.Status == ServiceStatusMessages.Success)
            {
                return new RequesterResponse<DataResponse<ResponseT>>
                {
                    Response = responseModel,
                    IsSuccess = true
                };
            }
            else
            {
                var errorMessageBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(responseModel.Message))
                {
                    errorMessageBuilder.AppendLine(responseModel.Message);
                }
                if (responseModel.ValidationMessages != null && responseModel.ValidationMessages.Any())
                {
                    foreach (var error in responseModel.ValidationMessages)
                    {
                        errorMessageBuilder.AppendLine(error);
                    }
                }

                _toastService.ShowError(errorMessageBuilder.ToString());
            }

            return new RequesterResponse<DataResponse<ResponseT>>
            {
                IsSuccess = false
            };
        }

        public async Task<RequesterResponse<DataResponse<ResponseT>>> PostAsync<RequestT, ResponseT>(string url, RequestT requestModel)
        {
            var result = await _httpClient.PostAsJsonAsync(url, requestModel);

            var responseModel = await result.Content.ReadFromJsonAsync<DataResponse<ResponseT>>();

            if (responseModel.Status == ServiceStatusMessages.Success)
            {
                return new RequesterResponse<DataResponse<ResponseT>>
                {
                    Response = responseModel,
                    IsSuccess = true
                };
            }
            else
            {
                var errorMessageBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(responseModel.Message))
                {
                    errorMessageBuilder.AppendLine(responseModel.Message);
                }
                if (responseModel.ValidationMessages != null && responseModel.ValidationMessages.Any())
                {
                    foreach (var error in responseModel.ValidationMessages)
                    {
                        errorMessageBuilder.AppendLine(error);
                    }
                }

                _toastService.ShowError(errorMessageBuilder.ToString());
            }

            return new RequesterResponse<DataResponse<ResponseT>>
            {
                IsSuccess = false
            };
        }
         
        public async Task<RequesterResponse<DataResponse<ResponseT>>> PutAsync<RequestT, ResponseT>(string url, RequestT requestModel)
        { 
            var result = await _httpClient.PutAsJsonAsync(url, requestModel, new System.Text.Json.JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull | System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
            });

            var r = await result.Content.ReadAsStringAsync();

            var responseModel = await result.Content.ReadFromJsonAsync<DataResponse<ResponseT>>();

            if (responseModel.Status == ServiceStatusMessages.Success)
            {
                return new RequesterResponse<DataResponse<ResponseT>>
                {
                    Response = responseModel,
                    IsSuccess = true
                };
            }
            else
            {
                var errorMessageBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(responseModel.Message))
                {
                    errorMessageBuilder.AppendLine(responseModel.Message);
                }
                if (responseModel.ValidationMessages != null && responseModel.ValidationMessages.Any())
                {
                    foreach (var error in responseModel.ValidationMessages)
                    {
                        errorMessageBuilder.AppendLine(error);
                    }
                }

                _toastService.ShowError(errorMessageBuilder.ToString());
            }

            return new RequesterResponse<DataResponse<ResponseT>>
            {
                IsSuccess = false
            };
        }

        public class RequesterResponse<T>
        {
            public bool IsSuccess { get; set; }
            public T Response { get; set; }
        }
    }
}