

using urbanMartAPI.Repositories;
using urbanMartAPI.SystemEntities;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartResponse> GetCart(string username)
    {
        Cart cart = await _cartRepository.GetCartAsync(username);
        return MapToCartResponse(cart);
    }

    public async Task<CartResponse> AddToCart(string username, CartRequest request)
    {
        Product product = await _productRepository.GetProductByIdAsync(request.ProductId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {request.ProductId} does not exist.");
        }

        CartItem cartItem = new CartItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = request.Quantity,
            UnitPrice = product.Price
        };

        Cart cart = await _cartRepository.AddToCartAsync(username, cartItem);
        return MapToCartResponse(cart);
    }

    private CartResponse MapToCartResponse(Cart cart)
    {
        return new CartResponse
        {
            Items = cart.CartItems.Select(i => new CartItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            TotalPrice = cart.CartItems.Sum(i => i.Quantity * i.UnitPrice)
        };
    }
}
