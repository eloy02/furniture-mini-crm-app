using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FurnitureMiniCrm.Services
{
    public class ProductsService : ServiceBase, IProductsService
    {
        private readonly Subject<ProductModel> _newProducts = new Subject<ProductModel>();

        public Task<IEnumerable<ProductModel>> GetProductsAsync()
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ProductModel>();

            var result = col.FindAll().ToList();

            return Task.FromResult(result.AsEnumerable());
        }

        public Task<ProductModel> GetProductAsync(int id)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ProductModel>();

            col.EnsureIndex(x => x.Id, unique: true);

            return Task.FromResult(col
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefault());
        }

        public Task SetProductAsync(ProductModel product)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ProductModel>();

            col.EnsureIndex(x => x.Id, true);

            if (col.Exists(x => x.Id == product.Id))
                col.Update(product.Id, product);
            else
                col.Insert(product);

            _newProducts.OnNext(product);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<ProductGroupModel>> GetProductGroupsAsync()
        {
            var groups = new List<ProductGroupModel>()
            {
                new ProductGroupModel()
                {
                    Id = 1,
                    Name = "Спальня"
                },

                new ProductGroupModel()
                {
                    Id = 2,
                    Name = "Кухня"
                },

                new ProductGroupModel()
                {
                    Id = 3,
                    Name = "Гостинная"
                },
            };

            return Task.FromResult(groups.AsEnumerable());
        }

        public IObservable<ProductModel> NewProducts => _newProducts;

        public Task<IEnumerable<ProductStatusModel>> GetProductStatusesAsync()
        {
            var statuses = new List<ProductStatusModel>()
            {
                new ProductStatusModel()
                {
                    Id = 1,
                    Name = "В продаже"
                },

                new ProductStatusModel()
                {
                    Id = 2,
                    Name = "Снят с продажи"
                }
            };

            return Task.FromResult(statuses.AsEnumerable());
        }
    }
}