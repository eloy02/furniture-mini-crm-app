using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurnitureMiniCrm.Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderModel>> GetOrdersAsync();

        Task<IEnumerable<OrderModel>> GetOrdersAsync(ClientModel client);

        Task<IEnumerable<OrderStatusModel>> GetOrderStatusesAsync();

        Task AddOrderStatusAsync(OrderStatusModel orderStatus);

        Task DeleteOrderStatusAsync(OrderStatusModel orderStatus);

        Task DeleteOrderStatusAsync(int id);

        Task<OrderModel> GetOrderAsync(int id);

        Task<OrderModel> CreateNewOrder();

        Task SetOrderAsync(OrderModel order);

        IObservable<OrderModel> NewOrders { get; }
    }
}