using System.Collections.Generic;
using System.Threading.Tasks;
using aspnetcoregraphql.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using aspnetcoregraphql.Models.Entities;
using System.Threading;

namespace aspnetcoregraphql.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private  List<Customer> _customers;
   
        public CustomerRepository()
        {
            _customers = new List<Customer>{
                new Customer()
                {
                    Id = 1,
                    Name = "Ahmed"
                },
                new Customer()
                {
                    Id = 2,
                    Name = "Mohamed"
                },
                new Customer()
                {
                    Id = 3,
                    Name = "Hamza"
                }
            };
        }   
        public Task<List<Customer>> CustomersAsync()
        {
            return Task.FromResult(_customers);
        }

        public Task<Customer> GetCustomerAsync(int id)
        {
            return Task.FromResult(_customers.FirstOrDefault(x => x.Id == id));
        }

        public Task<Dictionary<int, Customer>> GetCustomersByIdAsync(IEnumerable<int> customerIds, CancellationToken cancellationToken) 
        {
            var joinedCustomers  = _customers.Join(customerIds, c => c.Id, c => c, (c, _) => c);

		    return Task.FromResult(joinedCustomers.ToDictionary(u => u.Id));
        }  

    }
}
