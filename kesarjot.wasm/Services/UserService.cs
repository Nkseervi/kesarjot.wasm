using System.Net.Http;

namespace kesarjot.wasm.Services
{
    public class UserService : IUserService
	{
		private readonly HttpClient _httpClient;

		public UserService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task CreateUser(NewUserDto userDto)
		{
			try
			{
				var data = new
				{
					userDto.FirstName,
					userDto.LastName,
					userDto.EmailAddress,
					userDto.Password
				};

				var response = await _httpClient.PostAsJsonAsync("/Register", data);

				if (response.IsSuccessStatusCode)
				{
					//if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					//{
					//	return default(NewUserDto);
					//}

					//return await response.Content.ReadFromJsonAsync<NewUserDto>();
				}
				else
				{
					var message = await response.Content.ReadAsStringAsync();
					throw new Exception($"Http status code: {response.StatusCode} message: {message}");
				}
			}
			catch (Exception)
			{
				//Log exception
				throw;
			}
		}
	}
}
