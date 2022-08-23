using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PriceHunter.Web.Data.Base;
using PriceHunter.Web.Data.Login;
using PriceHunter.Web.Helpers.Constants;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace PriceHunter.Web.Helpers.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(
            ILocalStorageService localStorage,
            HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = new AuthenticationState(new ClaimsPrincipal());

            var tokenData = await _localStorage.GetItemAsync<GetTokenResponse>(AppConstants.TokenStorageKey);
            if (tokenData != null)
            {
                var tokenExpireDate = ConvertFromUnixTimestamp(tokenData.ExpiresIn.Value);
                if (tokenExpireDate < DateTime.UtcNow)
                {
                    await _localStorage.RemoveItemAsync(AppConstants.TokenStorageKey);

                    if (tokenData.RefreshTokenExpireDate > DateTime.UtcNow)
                    {
                        try
                        {
                            var response = await _httpClient.PostAsJsonAsync(AppConstants.V1ApiRefreshTokenUrl, new RefreshTokenRequest { Token = tokenData.RefreshToken });
                            var responseModel = await response.Content.ReadFromJsonAsync<DataResponse<GetTokenResponse>>();

                            await _localStorage.SetItemAsync(AppConstants.TokenStorageKey, responseModel.Data);
                            state = Authenticate(responseModel.Data.AccessToken);
                        }
                        catch (Exception)
                        {
                            //Ignored    
                        }
                    }
                }
                else
                {
                    state = Authenticate(tokenData.AccessToken);
                }
            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        private AuthenticationState Authenticate(string accessToken)
        {
            var claims = ParseClaimsFromJwt(accessToken);
            var identity = new ClaimsIdentity(claims, AppConstants.AuthenticationType);

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp); //
        }
    }
}
