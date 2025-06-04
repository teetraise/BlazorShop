using BlazorShop.Shared.Models;

namespace BlazorShop.Client.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductAsync(int id);
        Task<(bool Success, string ErrorMessage)> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}