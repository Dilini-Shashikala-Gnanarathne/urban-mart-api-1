
using urbanMartAPI.SystemEntities;

public interface ICartRepository
{
    Task<Cart> GetCartAsync(string username);
    Task<Cart> AddToCartAsync(string username, CartItem item);
}
