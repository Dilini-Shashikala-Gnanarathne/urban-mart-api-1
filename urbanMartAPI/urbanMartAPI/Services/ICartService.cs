public interface ICartService
{
    Task<CartResponse> GetCart(string username);
    Task<CartResponse> AddToCart(string username, CartRequest request);
}
