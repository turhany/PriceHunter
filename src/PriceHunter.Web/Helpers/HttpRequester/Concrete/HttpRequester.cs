using PriceHunter.Web.Data.Base;
using PriceHunter.Web.Helpers.Constants;
using System.Text;
using Blazored.Toast.Services;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using PriceHunter.Web.Data.Login;
using PriceHunter.Web.Helpers.HttpRequester.Abstract;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.Serialization.Formatters.Binary;

namespace PriceHunter.Web.Helpers.HttpRequester.Concrete
{
    public partial class HttpRequester : IHttpRequester
    {
        private HttpClient _httpClient { get; }
        private IToastService _toastService { get; }
        private ILocalStorageService _localStorage { get; }

        public HttpRequester(
            HttpClient httpClient,
            IToastService toastService,
            ILocalStorageService localStorage
            )
        {
            _httpClient = httpClient;
            _toastService = toastService;
            _localStorage = localStorage;
        }

        public async Task<RequesterResponse<DataResponse<ResponseT>>> GetAsync<ResponseT>(string url, bool includeToken)
        {
            HttpResponseMessage result;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                if (includeToken)
                {
                    var tokenData = await _localStorage.GetItemAsync<GetTokenResponse>(AppConstants.TokenStorageKey);
                    if (tokenData != null)
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
                    }
                }

                result = await _httpClient.SendAsync(requestMessage);
            }

            return await ProcessAsync<ResponseT>(result);
        }

        public async Task<RequesterResponse<DataResponse<ResponseT>>> PostAsync<RequestT, ResponseT>(string url, RequestT requestModel, bool includeToken)
        {
            HttpResponseMessage result;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (includeToken)
                {
                    var tokenData = await _localStorage.GetItemAsync<GetTokenResponse>(AppConstants.TokenStorageKey);
                    if (tokenData != null)
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
                    }
                }

                requestMessage.Content = JsonContent.Create(requestModel);
                result = await _httpClient.SendAsync(requestMessage);
            }

            return await ProcessAsync<ResponseT>(result);
        }

        public async Task<RequesterResponse<DataResponse<ResponseT>>> PutAsync<RequestT, ResponseT>(string url, RequestT requestModel, bool includeToken)
        {
            HttpResponseMessage result;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, url))
            {
                if (includeToken)
                {
                    var tokenData = await _localStorage.GetItemAsync<GetTokenResponse>(AppConstants.TokenStorageKey);
                    if (tokenData != null)
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
                    }
                }

                requestMessage.Content = JsonContent.Create(requestModel);
                result = await _httpClient.SendAsync(requestMessage);
            }

            return await ProcessAsync<ResponseT>(result);
        }

        private async Task<RequesterResponse<DataResponse<ResponseT>>> ProcessAsync<ResponseT>(HttpResponseMessage httpResponseMessage)
        {
            //var a = await httpResponseMessage.Content.ReadAsStringAsync();

            //DataResponse<ResponseT> responseModel = await httpResponseMessage.Content.ReadFromJsonAsync<DataResponse<ResponseT>>();

            //try
            //{
            //    responseModel = await httpResponseMessage.Content.ReadFromJsonAsync<DataResponse<ResponseT>>();
            //}
            //catch (System.Text.Json.JsonException ex)
            //{
            //    responseModel = (DataResponse<ResponseT>)await httpResponseMessage.Content.ReadFromJsonAsync<BaseResponse>();
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            var responseModel = await httpResponseMessage.Content.ReadFromJsonAsync<DataResponse<ResponseT>>();

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
    }
}