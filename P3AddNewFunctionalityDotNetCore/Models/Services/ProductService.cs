﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Models.Services
{
    public class ProductService : IProductService
    {
        private readonly ICart _cart;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IStringLocalizer<ProductService> _localizer;
        private readonly IHubContext<CartHub> _hubContext;

        public ProductService(ICart cart, IProductRepository productRepository,
            IOrderRepository orderRepository, IStringLocalizer<ProductService> localizer,IHubContext<CartHub> hubContext)
        {
            _cart = cart;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _localizer = localizer;
            _hubContext = hubContext;
        }
        public List<ProductViewModel> GetAllProductsViewModel()
        {
             
            IEnumerable<Product> productEntities = GetAllProducts();
            return MapToViewModel(productEntities);
        }

        private static List<ProductViewModel> MapToViewModel(IEnumerable<Product> productEntities)
        {
            List <ProductViewModel> products = new List<ProductViewModel>();
            foreach (Product product in productEntities)
            {
                products.Add(new ProductViewModel
                {
                    Id = product.Id,
                    Stock = product.Quantity.ToString(),
                    Price = product.Price.ToString(CultureInfo.InvariantCulture),
                    Name = product.Name,
                    Description = product.Description,
                    Details = product.Details
                });
            }

            return products;
        }

        public List<Product> GetAllProducts()
        {
            IEnumerable<Product> productEntities = _productRepository.GetAllProducts();
            return productEntities?.ToList();
        }

        public ProductViewModel GetProductByIdViewModel(int id)
        {
            List<ProductViewModel> products = GetAllProductsViewModel().ToList();
            return products.Find(p => p.Id == id);
        }


        public Product GetProductById(int id)
        {
            List<Product> products = GetAllProducts().ToList();
            return products.Find(p => p.Id == id);
        }

        public async Task<Product> GetProduct(int id)
        {
            var product = await _productRepository.GetProduct(id);
            return product;
        }

        public async Task<IList<Product>> GetProduct()
        {
            var products = await _productRepository.GetProduct();
            return products;
        }
        public void UpdateProductQuantities()
        {
            Cart cart = (Cart) _cart;
            foreach (CartLine line in cart.Lines)
            {
                _productRepository.UpdateProductStocks(line.Product.Id, line.Quantity);
            }
        }

        // A method to return ValidationMessages 
        public List<string> ValidateModel(ProductViewModel model)
        {   List<string> errorMessages = new List<string>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var errorResults = new List<ValidationResult>();

            // Calling the TryValidateObject
            var isValid = Validator.TryValidateObject(model, context, errorResults,true);
            foreach(var error in errorResults) {
                errorMessages.Add(error.ErrorMessage);
            }

            return errorMessages;

        }

        public void SaveProduct(ProductViewModel product)
        {
            var productToAdd = MapToProductEntity(product);
            _productRepository.SaveProduct(productToAdd);
        }
    
        private static Product MapToProductEntity(ProductViewModel product)
        {
            if (!double.TryParse(product.Price.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedPrice))
            {
                throw new FormatException($"Invalid price format: '{product.Price}'");
            }
            Product productEntity = new Product
            {
                Name = product.Name,
                Price = parsedPrice,
                Quantity = Int32.Parse(product.Stock),
               
                Description = product.Description,
                Details = product.Details
            };
            return productEntity;
        }

        public void DeleteProduct(int id)
        {  //SignalAir
           //Autres solutions 
           // TODO what happens if a product
           // has been added to a cart and has been later removed from the inventory ?
           // delete the product from the cart by using the specific method
           // => the choice is up to the student


            var product = GetProductById(id);

            if (product != null)
            {
                _cart.RemoveLine(GetProductById(id));

                _productRepository.DeleteProduct(id);
                // Notifier les clients via SignalR
                _hubContext.Clients.All.SendAsync("ProductUnavailable", id);
            }
            else
            {
                throw new ArgumentException("Produit introuvable !");
            }
          
        }
    }
}
