using kithub.api.models.Dtos;
using System.Net.Http;

namespace kesarjot.wasm.Services
{
    public class PaymentService : IPaymentService
    {
        #region Constructor
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public PaymentService(HttpClient client,
                                IConfiguration config)
        {
            _client = client;
            _config = config;
        }
        #endregion

        #region Generate Payment Link for the Order
        public async Task<string> GeneratePaymentLink(OrderDto orderDto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync<OrderDto>("api/Payment", orderDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    return await response.Content.ReadFromJsonAsync<string>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} Message -{message}");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Order details from order id
        public async Task<OrderDto> GetOrderDetails(int orderId)
        {
            try
            {
                var response = await _client.GetAsync($"api/Order/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(OrderDto);
                    }

                    return await response.Content.ReadFromJsonAsync<OrderDto>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} Message -{message}");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Create New Order
        public async Task<int> CreateNewOrder(OrderDto orderDto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync<OrderDto>("api/Order/CreateOrder", orderDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    return await response.Content.ReadFromJsonAsync<int>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} Message -{message}");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}
