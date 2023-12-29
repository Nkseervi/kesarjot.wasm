namespace kesarjot.wasm.Services.Contracts
{
    public interface IPaymentService
    {
        Task<string> GeneratePaymentLink(OrderDto orderDto);
        Task<OrderDto> GetOrderDetails(int orderId);
    }
}