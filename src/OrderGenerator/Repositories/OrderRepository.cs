using MongoDB.Driver;
using OrderGenerator.Interfaces.Repositories;
using OrderGenerator.Models;
using OrderGenerator.Models.Enum;
using OrderGenerator.Repositories.Context;

namespace OrderGenerator.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;
        
        public OrderRepository(MongoDbContext dbContext)
        {
            _collection = dbContext.GetCollection<Order>();
            var indexKeys = Builders<Order>.IndexKeys.Descending(o => o.OrderTime);
            var indexOptions = new CreateIndexOptions { Name = "OrderTimeDescIndex" };
            _collection.Indexes.CreateOne(new CreateIndexModel<Order>(indexKeys, indexOptions));
        }

        public async Task AddOrder(Order entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task<IEnumerable<Order>> GetAll() =>
            await _collection.Find(_ => true)
                         .SortByDescending(o => o.OrderTime)
                         .ToListAsync();


        public async Task UpdateOrderStatus(string id, OrderStatus status, string? rejectionReason)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.ClOrdID, id);

            var update = Builders<Order>.Update
                .Set(o => o.Status, status)
                .Set(o => o.RejectionReason, rejectionReason)
                .Set(o => o.UpdatedAt, DateTime.Now);

            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
