namespace kesarjot.wasm.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUserDto> Login(AuthenticationUserDto userForAuthentication);
        Task Logout();
    }
}