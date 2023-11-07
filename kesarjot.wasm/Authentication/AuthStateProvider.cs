using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace kesarjot.wasm.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
		private readonly IConfiguration _config;
		private readonly LoggedInUserDto _loggedInUserDto;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(HttpClient httpClient,
								 ILocalStorageService localStorage,
								 IConfiguration config,
                                 LoggedInUserDto loggedInUserDto)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
			_config = config;
			_loggedInUserDto = loggedInUserDto;
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

            bool isAuthenticated = await NotifyUserAuthentication(token);
            if(!isAuthenticated)
            {
                return _anonymous;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

			return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(JwtParser.ParseClaimsFromJWT(token), 
                    "jwtAuthType")));
        }

        public async Task<bool> NotifyUserAuthentication(string token)
        {
            Task<AuthenticationState> authState;
			bool isAuthenticatedOutput;
			try
            {
                #region GetLoggedInUserInfo
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                using (HttpResponseMessage response = await _httpClient.GetAsync("/api/User"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<LoggedInUserDto>();
                        _loggedInUserDto.CreateDate = result.CreateDate;
                        _loggedInUserDto.EmailAddress = result.EmailAddress;
                        _loggedInUserDto.FirstName = result.FirstName;
                        _loggedInUserDto.LastName = result.LastName;
                        _loggedInUserDto.Token = token;
                        _loggedInUserDto.Id = result.Id;
                        _loggedInUserDto.CartId = result.CartId;
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                } 
                #endregion

                var authenticatedUser = new ClaimsPrincipal(
                        new ClaimsIdentity(JwtParser.ParseClaimsFromJWT(token),
                        "jwtAuthType"));
                authState = Task.FromResult(new AuthenticationState(authenticatedUser));
                NotifyAuthenticationStateChanged(authState);
                isAuthenticatedOutput = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await NotifyUserLogout();
                isAuthenticatedOutput= false;
            }
            return isAuthenticatedOutput;
        }

        public async Task NotifyUserLogout()
		{
			string authTokenStorageKey = _config["authTokenStorageKey"];
            await _localStorage.RemoveItemAsync(authTokenStorageKey);
            var authState = Task.FromResult(_anonymous);
            #region LogOffUser
            _httpClient.DefaultRequestHeaders.Clear();
            #endregion
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _loggedInUserDto.ResetUserModel();
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
