namespace kesarjot.wasm.Services.Contracts
{
    public interface IUserService
    {
        Task CreateUser(NewUserDto userDto);
    }
}