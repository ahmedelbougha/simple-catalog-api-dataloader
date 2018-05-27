using System.Linq;
using aspnetcoregraphql.Data.Repositories;
using aspnetcoregraphql.Models.Entities;
using GraphQL.DataLoader;
using GraphQL.Types;
using System;
using System.Collections.Generic;

namespace aspnetcoregraphql.Models.Types
{
    public class CustomerType : ObjectGraphType<Customer>
    {
        public CustomerType(IOrderRepository orderRepository, IDataLoaderContextAccessor accessor)
        {
            Field(x => x.Id).Description("Customer id.");
            Field(x => x.Name, nullable: true).Description("Customer name.");

            Field<ListGraphType<OrderType>>(
                "orders", 
                resolve: context => orderRepository.GetOrdersWithByCustomerIdAsync(context.Source.Id).Result.ToList()
            );

            Field<ListGraphType<OrderType>, IEnumerable<Order>>()
            .Name("ordersloader")
            .ResolveAsync(context =>
            {
                if (accessor.Equals(null)) {
                    throw new Exception("accessor is null!");
                }
                if (accessor.Context.Equals(null)) {
                    throw new Exception("context is null!");
                }
                // Get or add a collection batch loader with the key "GetOrdersCollectionWithByCustomerIdAsync"
                // The loader will call GetOrdersCollectionWithByCustomerIdAsync for each batch of keys
                var loader = accessor.Context.GetOrAddCollectionBatchLoader<int, Order>("GetOrdersCollectionWithByCustomerIdAsync", orderRepository.GetOrdersCollectionWithByCustomerIdAsync);

                // Add this Customer Id to the pending keys to fetch
                // The task will complete once the GetOrdersCollectionWithByCustomerIdAsync() returns with the batched results
                return loader.LoadAsync(context.Source.Id);
            });            
        }
    }
}