using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using ProductService = P3AddNewFunctionalityDotNetCore.Models.Services.ProductService;
using Microsoft.AspNetCore.SignalR;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductControllerTest
    {
        [Fact]
        public void AddProduct_Should_Add_Product_To_Database()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<P3Referential>()
                            .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                            .Options;

            // Création du contexte avec une base en mémoire
            var config = new Mock<IConfiguration>();
            var context = new P3Referential(options, config.Object);
            var productRepository = new ProductRepository(context);
            ProductService productService = new ProductService(It.IsAny<ICart>(),
                                                    productRepository,
                                                    It.IsAny<IOrderRepository>(),
                                                    It.IsAny<IStringLocalizer<Models.Services.ProductService>>(),It.IsAny<IHubContext<CartHub>>());
            var controller = new ProductController(productService, null);

            var newProduct = new ProductViewModel
            {
                Name = "Test Product",
                Price = "20.5", // Prix valide
                Stock = "10", // Stock valide
                Description = "This is a test product",
                Details = "Test details"
            };

            // Act
            var result = controller.Create(newProduct) as RedirectToActionResult;

            // Assert
            // Vérifier que l'action redirige bien
            Assert.NotNull(result);
            Assert.Equal("Admin", result.ActionName);

            // Vérifier que le produit a bien été ajouté dans la base
            var addedProduct = context.Set<Product>().FirstOrDefault(p => p.Name == "Test Product");
            Assert.NotNull(addedProduct);
            Assert.Equal("Test Product", addedProduct.Name);
            Assert.Equal(20.0, addedProduct.Price); // Vérifie que le prix est bien converti
            Assert.Equal(10, addedProduct.Quantity); // Vérifie la quantité
        }



        [Fact]
        public void DeleteProduct_Should_Remove_Product_From_Database()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<P3Referential>()
                           .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                           .Options;

            // Création du contexte avec une base en mémoire
            var config = new Mock<IConfiguration>();
            var context = new P3Referential(options, config.Object);
            var productRepository = new ProductRepository(context);
            Cart cart = new Cart();
            ProductService productService = new ProductService(cart,
                                                           productRepository,
                                                           It.IsAny<IOrderRepository>(),
                                                           It.IsAny<IStringLocalizer<Models.Services.ProductService>>(),It.IsAny<IHubContext<CartHub>>());
            var controller = new ProductController(productService, null);

            // Créer un produit à supprimer
            var newProduct = new Product
            {
                Name = "Product to Delete",
                Price = 20.0,
                Quantity = 10,
                Description = "Product to be deleted",
                Details = "Details about the product"
            };

            // Ajouter le produit à la base de données
            context.Set<Product>().Add(newProduct);
            context.SaveChanges();

            // Vérifier que le produit est bien ajouté
            var addedProduct = context.Set<Product>().FirstOrDefault(p => p.Name == "Product to Delete");
            Assert.NotNull(addedProduct);

            // Act

            var result = controller.DeleteProduct(addedProduct.Id) as RedirectToActionResult;

            // Assert

            Assert.NotNull(result);
            Assert.Equal("Admin", result.ActionName);


            var deletedProduct = context.Set<Product>().FirstOrDefault(p => p.Id == addedProduct.Id);
            Assert.Null(deletedProduct);
        }
    }
}
