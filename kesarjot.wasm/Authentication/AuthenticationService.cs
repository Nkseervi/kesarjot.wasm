using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace kesarjot.wasm.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _config;
        private readonly string _authTokenStorageKey = string.Empty;

        public AuthenticationService(HttpClient httpClient,
                               AuthenticationStateProvider authStateProvider,
                               ILocalStorageService localStorage,
                               IConfiguration config)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _config = config;
            _authTokenStorageKey = _config["authTokenStorageKey"];
        }

        public async Task<AuthenticatedUserDto> Login(AuthenticationUserDto userForAuthentication)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userForAuthentication.Email),
                new KeyValuePair<string, string>("password", userForAuthentication.Password)
            });

            string api = _config["apiLocation"] + _config["tokenEndpoint"];
            var authResult = await _httpClient.PostAsync(api, data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (authResult.IsSuccessStatusCode == false)
            {
                return null;
            }
            var result = System.Text.Json.JsonSerializer.Deserialize<AuthenticatedUserDto>(authContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await _localStorage.SetItemAsync(_authTokenStorageKey, result.Access_Token);

            await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Access_Token);

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.Access_Token);
            return result;
        }

        public async Task Logout()
        {
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

    }
}
