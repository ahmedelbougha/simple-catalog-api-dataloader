using System.Collections.Generic;
using System.Threading.Tasks;
using aspnetcoregraphql.Models;
using aspnetcoregraphql.Models.Entities;
using System.Threading;

namespace aspnetcoregraphql.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> CustomersAsync();
        Task<Customer> GetCustomerAsync(int id);
        Task<Dictionary<int, Customer>> GetCustomersByIdAsync(IEnumerable<int> customerIds, CancellationToken cancellationToken);
    }
}