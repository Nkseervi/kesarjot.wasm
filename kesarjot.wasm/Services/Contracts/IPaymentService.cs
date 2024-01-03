
namespace kesarjot.wasm.Services.Contracts
{
    public interface IPaymentService
    {
        Task<int> CreateNewOrder(OrderDto orderDto);
        Task<string> GeneratePaymentLink(OrderDto orderDto);
        Task<IEnumerable<OrderDto>> GetAllOrders();
        Task<OrderDto> GetOrderDetails(int orderId);
    }
}