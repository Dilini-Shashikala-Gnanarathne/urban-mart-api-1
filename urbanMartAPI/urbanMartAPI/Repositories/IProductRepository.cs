

using urbanMartAPI.SystemEntities;

namespace urbanMartAPI.Repositories;
public interface IProductRepository
{
    //List<Product> GetAllProducts();
    //Product GetProductById(long id);
    //Product CreateProduct(Product product);

    Task<Product> GetProductByIdAsync(long productId);
}
