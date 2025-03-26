

using urbanMartAPI.SystemEntities;

namespace urbanMartAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync(string username);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(string orderId);

        Task<Order> UpdateOrderAsync(Order order);
    }
}

