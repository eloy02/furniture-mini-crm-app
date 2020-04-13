using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurnitureMiniCrm.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductModel>> GetProductsAsync();

        Task<ProductModel> GetProductAsync(int id);

        Task SetProductAsync(ProductModel product);

        Task<IEnumerable<ProductGroupModel>> GetProductGroupsAsync();

        Task AddProductGroupAsync(ProductGroupModel productGroup);

        Task DeleteProductGroupAsync(ProductGroupModel productGroup);

        Task DeleteProductGroupAsync(int productGroupId);

        Task<IEnumerable<ProductStatusModel>> GetProductStatusesAsync();

        IObservable<ProductModel> NewProducts { get; }
    }
}