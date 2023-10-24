namespace kesarjot.wasm.Pages
{
    public partial class DisplayProducts
    {
        [Parameter]
        public IEnumerable<ProductDto> Products { get; set; }
    }
}