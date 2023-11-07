namespace kesarjot.wasm.Services
{
    public class ManageProductsLocalStorageService : IManageProductsLocalStorageService
    {
        private readonly ILocalStorageService localStorageService;
        private readonly IProductService productService;
        private readonly IConfiguration _config;

        public ManageProductsLocalStorageService(ILocalStorageService localStorageService,
                                                 IProductService productService,
                                                 IConfiguration config)
        {
            this.localStorageService = localStorageService;
            this.productService = productService;
            _config = config;
        }

        public async Task<IEnumerable<ProductDto>> GetCollection()
        {
            return await this.localStorageService.GetItemAsync<IEnumerable<ProductDto>>(_config["productsStorageKey"])
                    ?? await AddCollection();
        }

        public async Task RemoveCollection()
        {
            await this.localStorageService.RemoveItemAsync(_config["productsStorageKey"]);
        }

        private async Task<IEnumerable<ProductDto>> AddCollection()
        {
            var productCollection = await this.productService.GetItems();

            if (productCollection != null)
            {
                await this.localStorageService.SetItemAsync(_config["productsStorageKey"], productCollection);
            }

            return productCollection;

        }

    }
}
