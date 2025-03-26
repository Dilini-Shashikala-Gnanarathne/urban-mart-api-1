using Microsoft.EntityFrameworkCore;
using urbanMartAPI.SystemEntities;

public class ProductService : IProductService
{
    private readonly EcommerceDbContext _context;
    private readonly CustomContext _customContext;

    public ProductService(EcommerceDbContext context, CustomContext customContext)
    {
        _context = context;
        _customContext = customContext;
    }

    public async Task<List<ProductResponse>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products.Select(MapToProductResponse).ToList();
    }

    public async Task<ProductResponse> GetProductByIdAsync(long id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        return product != null ? MapToProductResponse(product) : null;
    }

    public async Task<ProductResponse> CreateProductAsync(ProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            Category = request.Category,
            Images = request.Category,
            CreatedBy = _customContext.Username
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return MapToProductResponse(product);
    }

    private ProductResponse MapToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
         
        };
    }
}
