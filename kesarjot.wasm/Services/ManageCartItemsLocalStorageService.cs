namespace kesarjot.wasm.Services
{
    public class ManageCartItemsLocalStorageService : IManageCartItemsLocalStorageService
    {
        private readonly ILocalStorageService localStorageService;
        private readonly IShoppingCartService shoppingCartService;
		private readonly LoggedInUserDto _loggedInUserDto;
        private readonly IConfiguration _config;

        public ManageCartItemsLocalStorageService(ILocalStorageService localStorageService,
                                                  IShoppingCartService shoppingCartService,
                                                  LoggedInUserDto loggedInUserDto,
                                                  IConfiguration config)
        {
            this.localStorageService = localStorageService;
            this.shoppingCartService = shoppingCartService;
			_loggedInUserDto = loggedInUserDto;
            _config = config;
        }

        public async Task<List<CartItemDto>> GetCollection()
        {
            return await this.localStorageService.GetItemAsync<List<CartItemDto>>(_config["cartItemsStorageKey"])
                    ?? await AddCollection();
        }

        public async Task RemoveCollection()
        {
            await this.localStorageService.RemoveItemAsync(_config["cartItemsStorageKey"]);
        }

        public async Task SaveCollection(List<CartItemDto> cartItemDtos)
        {
            await this.localStorageService.SetItemAsync(_config["cartItemsStorageKey"], cartItemDtos);
        }

        private async Task<List<CartItemDto>> AddCollection()
        {
            var shoppingCartCollection = await this.shoppingCartService.GetItems("8e9c9233-2b0f-40f6-8ca9-a331b33748ad");
            //var shoppingCartCollection = await this.shoppingCartService.GetItems(_loggedInUserDto.Id);

            if (shoppingCartCollection != null)
            {
                await this.localStorageService.SetItemAsync(_config["cartItemsStorageKey"], shoppingCartCollection);
            }

            return shoppingCartCollection;

        }

    }
}
