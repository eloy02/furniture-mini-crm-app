using System;
using System.Collections.Generic;

namespace FurnitureMiniCrm.Services
{
    public class OrderStatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OrderProductModel
    {
        public int Id { get; set; }
        public ProductModel Product { get; set; }
        public int Count { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public OrderStatusModel Status { get; set; }
        public ClientModel Client { get; set; }
        public List<OrderProductModel> Products { get; set; }
        public decimal Sum { get; set; }
    }
}