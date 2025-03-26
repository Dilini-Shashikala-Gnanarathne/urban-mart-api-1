
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using urbanMartAPI.SystemEntities;

public class CartRepository : ICartRepository
{
    private readonly EcommerceDbContext _context;

    public CartRepository(EcommerceDbContext context)
    {
        _context = context;
    }

    // Retrieve the user's cart asynchronously
    public async Task<Cart> GetCartAsync(string username)
    {
        try
        {
            // Check if the cart exists for the user in the database
            Cart cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserNic == username);

            // If cart does not exist, create a new cart for the user
            if (cart == null)
            {
                cart = new Cart
                {
                    UserNic = username,
                    CreatedBy = username,
                    CartItems = new List<CartItem>()
                };

                // Add the new cart to the database
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Persist the cart to the database
            }

            return cart;
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., log and rethrow
            throw new InvalidOperationException("Failed to retrieve cart", ex);
        }
    }

    // Add an item to the user's cart asynchronously
    public async Task<Cart> AddToCartAsync(string username, CartItem item)
    {
        try
        {
            // Retrieve the user's cart
            Cart cart = await GetCartAsync(username);

            // Check if the item already exists in the cart
            CartItem existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                // If item exists, update the quantity
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                // If item doesn't exist, add a new item
                cart.CartItems.Add(item);
            }

            // Save changes to the database
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return cart;
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., log and rethrow
            throw new InvalidOperationException("Failed to add item to cart", ex);
        }
    }
}
