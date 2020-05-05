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
        public double TotalPrice { get; set; }
        public string Size { get; set; }
    }

    public class CustomOrderProductModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Цена продажи
        /// </summary>
        public double SellPrice { get; set; }

        /// <summary>
        /// Ед. изм
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Размеры
        /// </summary>
        public string Size { get; set; }

        public int Count { get; set; }
        public double TotalPrice { get; set; }
    }

    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public OrderStatusModel Status { get; set; }
        public ClientModel Client { get; set; }

        /// <summary>
        /// Товары из справочника
        /// </summary>
        public List<OrderProductModel> Products { get; set; }

        /// <summary>
        /// Индивидуальный заказ
        /// </summary>
        public List<CustomOrderProductModel> CustomProducts { get; set; }

        public double Sum { get; set; }
    }
}