using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using LiteDB;

namespace FurnitureMiniCrm.Services
{
    public class OrdersService : ServiceBase, IOrdersService
    {
        private readonly Subject<OrderModel> _newOrders = new Subject<OrderModel>();

        public Task<IEnumerable<OrderModel>> GetOrdersAsync() =>
            Task.FromResult(Get<OrderModel>()
                                .Where(x => x.Status != null));

        public IObservable<OrderModel> NewOrders => _newOrders;

        public Task<IEnumerable<OrderStatusModel>> GetOrderStatusesAsync()
        {
            var statuses = new List<OrderStatusModel>()
            {
                new OrderStatusModel()
                {
                    Id = 1,
                    Name = "Предварительный заказ"
                },

                new OrderStatusModel()
                {
                    Id = 2,
                    Name = "В работе"
                },

                new OrderStatusModel()
                {
                    Id = 3,
                    Name = "Завершен"
                },
            };

            return Task.FromResult(statuses.AsEnumerable());
        }

        public Task<OrderModel> GetOrderAsync(int id)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<OrderModel>();

            col.EnsureIndex(x => x.Id, unique: true);

            return Task.FromResult(col.FindOne(x => x.Id == id));
        }

        public Task SetOrderAsync(OrderModel order)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<OrderModel>();

            col.EnsureIndex(x => x.Id, unique: true);

            if (col.Exists(x => x.Id == order.Id))
                col.Update(order);
            else
                col.Insert(order);

            _newOrders.OnNext(order);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<OrderModel>> GetOrdersAsync(ClientModel client)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<OrderModel>();

            col.EnsureIndex(x => x.Client.Id);

            var orders = col
                .Query()
                .Where(x => x.Client != null)
                .Where(x => x.Client.Id == client.Id)
                .ToList();

            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<OrderModel> CreateNewOrder()
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<OrderModel>();

            col.DeleteMany(x => x.Status == null);

            var count = col.Count();

            var newOrder = new OrderModel()
            {
                Id = count + 1,
                CreateDate = DateTime.Now
            };

            col.Insert(newOrder);

            return Task.FromResult(newOrder);
        }

        public Task AddOrderStatusAsync(OrderStatusModel orderStatus)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<OrderStatusModel>();

            col.EnsureIndex(x => x.Id, unique: true);

            col.Insert(orderStatus);

            return Task.CompletedTask;
        }

        public Task DeleteOrderStatusAsync(OrderStatusModel orderStatus) =>
            DeleteOrderStatusAsync(orderStatus.Id);

        public Task DeleteOrderStatusAsync(int id)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<OrderStatusModel>();

            col.EnsureIndex(x => x.Id, unique: true);

            col.Delete(id);

            return Task.CompletedTask;
        }
    }
}