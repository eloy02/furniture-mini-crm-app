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

        Task<OrderModel> GetOrderAsync(int id);

        Task SetOrderAsync(OrderModel order);

        IObservable<OrderModel> NewOrders { get; }
    }
}