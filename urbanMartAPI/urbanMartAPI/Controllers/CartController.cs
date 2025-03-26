
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using urbanMartAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor)
    {
        _cartService = cartService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [PermissionAuthorization(Permissions.ViewCart)]
    public async Task<IActionResult> GetCart()
    {
        string username = _httpContextAccessor.HttpContext.User.Identity.Name;
        CartResponse cart = await _cartService.GetCart(username);
        return Ok(ApiResponse<CartResponse>.SuccessResponse("Cart retrieved successfully", cart));
    }

    [HttpPost]
    [PermissionAuthorization(Permissions.AddToCart)]
    public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
    {
        string username = _httpContextAccessor.HttpContext.User.Identity.Name;
        CartResponse cart = await _cartService.AddToCart(username, request);

        if (cart == null)
        {
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse("Failed to add item to cart"));
        }

        return Ok(ApiResponse<CartResponse>.SuccessResponse("Item added to cart successfully", cart));
    }
}
