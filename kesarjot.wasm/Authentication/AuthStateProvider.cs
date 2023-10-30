using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace kesarjot.wasm.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
		private readonly IConfiguration _config;
		private readonly AuthenticationState _anonymous;

        public AuthStateProvider(HttpClient httpClient,
								 ILocalStorageService localStorage,
								 IConfiguration config)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
			_config = config;
			_anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string authTokenStorageKey = _config["authTokenStorageKey"];
            var token = await _localStorage.GetItemAsync<string>(authTokenStorageKey);

            if(string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJWT(token), "jwtAuthType")));
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(
                new ClaimsIdentity(JwtParser.ParseClaimsFromJWT(token),
                "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);

        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
