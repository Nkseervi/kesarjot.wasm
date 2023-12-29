namespace kesarjot.wasm.Pages
{
    public partial class Checkout
    {
        [Inject]
        public IJSRuntime Js { get; set; }
        protected IEnumerable<CartItemDto> ShoppingCartItems { get; set; }
        protected int TotalQty { get; set; }
        protected string PaymentDescription { get; set; }
        protected decimal PaymentAmount { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }


        [Inject]
        public IPaymentService PaymentsManager { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }
        protected string DisplayButtons { get; set; } = "block";
        ShippingAddressDto model = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ShoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();
                if (ShoppingCartItems != null && ShoppingCartItems.Count() > 0)
                {
                    Guid orderGuid = Guid.NewGuid();
                    PaymentAmount = ShoppingCartItems.Sum(p => p.TotalPrice);
                    TotalQty = ShoppingCartItems.Sum(p => p.Qty);
                    PaymentDescription = $"O_{orderGuid}";
                }
                else
                {
                    DisplayButtons = "none";
                }
            }
            catch (Exception)
            {
                //Log exception
                throw;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    await Js.InvokeVoidAsync("initPayPalButton");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnValidSubmit()
        {
            try
            {
                OrderDto orderDto = new()
                {
                    Amount = Convert.ToInt32(PaymentAmount),
                };
                var redirectUrl = await PaymentsManager.GeneratePaymentLink(orderDto);

                NavManager.NavigateTo(redirectUrl);

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}