namespace FurnitureMiniCrm.Services
{
    public class ProductGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductStatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Группа продукта
        /// </summary>
        public ProductGroupModel Group { get; set; }

        /// <summary>
        /// Статус продукта
        /// </summary>
        public ProductStatusModel Status { get; set; }

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

        /// <summary>
        /// Цвет
        /// </summary>
        public string Color { get; set; }
    }
}