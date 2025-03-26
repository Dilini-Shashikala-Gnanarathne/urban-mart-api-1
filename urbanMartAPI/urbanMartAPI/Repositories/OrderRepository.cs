
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using urbanMartAPI.Repositories;
using urbanMartAPI.SystemEntities;

namespace urbanMartAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceDbContext  _context;

        public OrderRepository(EcommerceDbContext  context)
        {
            _context = context;
        }

        // Fetch all orders asynchronously
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        // Fetch orders by a specific user asynchronously
        public async Task<List<Order>> GetUserOrdersAsync(string username)
        {
            return await _context.Orders
                                  .Where(o => o.CreatedBy == username)
                                  .ToListAsync();
        }

        // Fetch a single order by its order ID asynchronously
        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                                 .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // Create a new order and save it asynchronously
        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(); // Save changes asynchronously
            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("Order was modified by another user.hhhhhhhhh");
            }
        }
    }
}
