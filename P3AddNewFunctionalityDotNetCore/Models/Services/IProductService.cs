using System.Collections.Generic;
using System.Threading.Tasks;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.ComponentModel.DataAnnotations;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Models.Services
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        List<ProductViewModel> GetAllProductsViewModel();
        Product GetProductById(int id);
        ProductViewModel GetProductByIdViewModel(int id);
        void UpdateProductQuantities();
        void SaveProduct(ProductViewModel product);
        void DeleteProduct(int id);
        List<string> ValidateModel(ProductViewModel product);
        Task<Product> GetProduct(int id);
        Task<IList<Product>> GetProduct();
    }
}
