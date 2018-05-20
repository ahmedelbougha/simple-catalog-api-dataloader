using System.Linq;
using aspnetcoregraphql.Data.Repositories;
using aspnetcoregraphql.Models.Entities;
using GraphQL.DataLoader;
using GraphQL.Types;
using System;

namespace aspnetcoregraphql.Models.Types
{
    public class CustomerType : ObjectGraphType<Customer>
    {
        public CustomerType(IDataLoaderContextAccessor accessor, IOrderRepository orderRepository,ICustomerRepository customerRepository)
        {
            Field(x => x.Id).Description("Customer id.");
            Field(x => x.Name, nullable: true).Description("Customer name.");

            Field<ListGraphType<OrderType>>(
                "orders", 
                resolve: context => orderRepository.GetOrdersWithByCustomerIdAsync(context.Source.Id).Result.ToList()
            );

            Field<CustomerType, Customer>()
            .Name("customers1")
            .ResolveAsync(context =>
            {

                if (accessor.Equals(null)) {
                    throw new Exception("accessor is null!");
                }
                if (accessor.Context.Equals(null)) {
                    throw new Exception("context is null!");
                }
                // Get or add a batch loader with the key "GetCustomersById"
                // The loader will call GetCustomersByIdAsync for each batch of keys
                var loader = accessor.Context.GetOrAddBatchLoader<int, Customer>("GetCustomersById", customerRepository.GetCustomersByIdAsync);

                // Add this CustomerId to the pending keys to fetch
                // The task will complete once the GetCustomersByIdAsync() returns with the batched results
                return loader.LoadAsync(context.Source.Id);
            });            
        }
    }
}