using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aspnetcoregraphql.Models;
using aspnetcoregraphql.Models.Entities;

namespace aspnetcoregraphql.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> OrdersAsync();
        Task<List<Order>> GetOrdersWithByCustomerIdAsync(int customerId);
        Task<ILookup<int, Order>> GetOrdersCollectionWithByCustomerIdAsync(IEnumerable<int> customerIds, CancellationToken cancellationToken);       
        Task<Order> GetOrderAsync(int id);    
        Task<Order> CreateAsync(Order order);
        Task<Order> StartAsync(int orderId);
        int GetOrdersCount(); 
    }
}