using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PriceHunter.Web.Data.Login;
using System.Security.Claims;
using System.Text.Json;

namespace PriceHunter.Web.Helpers
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //var sampleJwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjdlZjIxNjQwLTI2ZmYtNDMwZS04MGMxLThjYTQ5ZmQ0ZjY0YiIsIm5hbWUiOiJUw7xyaGFuIFnEsWxkxLFyxLFtIiwiZW1haWwiOiJ5aWxkaXJpbXR1cmhhbkBnbWFpbC5jb20iLCJyb2xlIjoiUm9vdCIsIm5iZiI6MTY2MTA4NDExNCwiZXhwIjoxNjYxMTA1NzE0LCJpYXQiOjE2NjEwODQxMTR9.DZquyGGrh4B_KSzFK7MtdYoLpFxN1BdnZ8e1RkOKp9g";
            //var jwtClaims = ParseClaimsFromJwt(sampleJwtToken);
            //var jwtIdentity = new ClaimsIdentity(jwtClaims, "jwt");

            //var jwtUser = new ClaimsPrincipal(jwtIdentity);
            //var jwtState = new AuthenticationState(jwtUser);

            //NotifyAuthenticationStateChanged(Task.FromResult(jwtState));

            //return jwtState;

            var state = new AuthenticationState(new ClaimsPrincipal()); 

            var tokenData = await _localStorage.GetItemAsync<GetTokenResponse>(AppConstants.TokenStorageKey);
            if (tokenData != null)
            {
                //TODO: Add token expire check and refresh token logic

                var claims = ParseClaimsFromJwt(tokenData.AccessToken);
                var identity = new ClaimsIdentity(claims, AppConstants.AuthenticationType);

                var user = new ClaimsPrincipal(identity);
                state = new AuthenticationState(user);
            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
