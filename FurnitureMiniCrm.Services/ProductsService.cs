using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using LiteDB;

namespace FurnitureMiniCrm.Services
{
    public class ProductsService : ServiceBase, IProductsService
    {
        private readonly Subject<ProductModel> _newProducts = new Subject<ProductModel>();

        public IObservable<ProductModel> NewProducts => _newProducts;

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
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ProductGroupModel>();

            var groups = col.FindAll().ToList();

            return Task.FromResult(groups.AsEnumerable());
        }

        public Task<IEnumerable<ProductStatusModel>> GetProductStatusesAsync()
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ProductStatusModel>();

            var statuses = col.FindAll().ToList();

            return Task.FromResult(statuses.AsEnumerable());
        }

        public Task AddProductGroupAsync(ProductGroupModel productGroup)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<ProductGroupModel>();

            col.Insert(productGroup);

            return Task.CompletedTask;
        }

        public Task DeleteProductGroupAsync(ProductGroupModel productGroup) =>
            DeleteProductGroupAsync(productGroup.Id);

        public Task DeleteProductGroupAsync(int productGroupId)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ProductGroupModel>();

            if (!col.Exists(x => x.Id == productGroupId))
                throw new Exception("Item not found");

            col.Delete(productGroupId);

            return Task.CompletedTask;
        }

        public Task AddProductStatusAsync(ProductStatusModel productStatus)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<ProductStatusModel>();

            col.Insert(productStatus);

            return Task.CompletedTask;
        }

        public Task DeleteProductStatusAsync(ProductStatusModel productStatus) =>
            DeleteProductStatusAsync(productStatus.Id);

        public Task DeleteProductStatusAsync(int productStatusId)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ProductStatusModel>();

            if (!col.Exists(x => x.Id == productStatusId))
                throw new Exception("Item not found");

            col.Delete(productStatusId);

            return Task.CompletedTask;
        }
    }
}