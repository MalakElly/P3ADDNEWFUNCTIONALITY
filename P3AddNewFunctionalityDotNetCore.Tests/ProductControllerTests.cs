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
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using ProductService = P3AddNewFunctionalityDotNetCore.Models.Services.ProductService;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Host;

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
                Price = "20", // Prix valide
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
            var config = new Mock<IConfiguration>();
            var context = new P3Referential(options, config.Object);

            // ✅ Création des mocks correctement
            var productRepository = new ProductRepository(context);
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockLocalizer = new Mock<IStringLocalizer<Models.Services.ProductService>>();
            var mockHubContext = new Mock<IHubContext<CartHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            // ⚠️ Utiliser SendCoreAsync au lieu de SendAsync pour éviter l'erreur Moq
            mockClientProxy
                .Setup(proxy => proxy.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            // Retourner le mock du client proxy pour tous les clients
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            mockHubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);
            var cart = new Cart();

            // ✅ Correction : utilisation des mocks et non de `It.IsAny<T>()`
            var productService = new ProductService(cart,
                                                    productRepository,
                                                    mockOrderRepository.Object,
                                                    mockLocalizer.Object,
                                                    mockHubContext.Object);

            var controller = new ProductController(productService, null);

            // ✅ Créer un produit et le sauvegarder
            var newProduct = new Product
            {
                
                Name = "Product to Delete",
                Price = 20,
                Quantity = 10,
                Description = "Product to be deleted",
                Details = "Details about the product"
            };

            context.Set<Product>().Add(newProduct);
            context.SaveChanges();

            // Vérifier que le produit a bien été ajouté
            var addedProduct = context.Set<Product>().FirstOrDefault(p => p.Id == newProduct.Id);
            Assert.NotNull(addedProduct);

            // Act 
            var result = controller.DeleteProduct(addedProduct.Id) as RedirectToActionResult;

            //  Assert : Vérifier que la suppression fonctionne
            Assert.NotNull(result);
            Assert.Equal("Admin", result.ActionName);

            var deletedProduct = context.Set<Product>().FirstOrDefault(p => p.Id == addedProduct.Id);
            Assert.Null(deletedProduct);
        }


        //[Fact]
        //public void DeleteProduct_Should_Remove_Product_From_Database()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<P3Referential>()
        //                   .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
        //                   .Options;

        //    // Création du contexte avec une base en mémoire
        //    var config = new Mock<IConfiguration>();
        //    var context = new P3Referential(options, config.Object);
        //    var productRepository = new ProductRepository(context);
        //    var mockHubContext = new Mock<IHubContext<CartHub>>();
        //    mockHubContext
        //        .Setup(m => m.Clients.All.SendAsync(It.IsAny<string>(), It.IsAny<int>(), default))
        //        .Returns(Task.CompletedTask);

           
        //    Cart cart = new Cart();
        //    ProductService productService = new ProductService(cart,
        //                                                   productRepository,
        //                                                   It.IsAny<IOrderRepository>(),
        //                                                   It.IsAny<IStringLocalizer<Models.Services.ProductService>>(), mockHubContext.Object);
        //    var controller = new ProductController(productService, null);

        //    // Créer un produit à supprimer
        //    var newProduct = new Product
        //    {
        //        Name = "Product to Delete",
        //        Price = 20,
        //        Quantity = 10,
        //        Description = "Product to be deleted",
        //        Details = "Details about the product"
        //    };

        //    // Ajouter le produit à la base de données
        //    context.Set<Product>().Add(newProduct);
        //    context.SaveChanges();

        //    // Vérifier que le produit est bien ajouté
        //    var addedProduct = context.Set<Product>().FirstOrDefault(p => p.Name == "Product to Delete");
        //    Assert.NotNull(addedProduct);

        //    // Act

        //    var result = controller.DeleteProduct(addedProduct.Id) as RedirectToActionResult;

        //    // Assert

        //    Assert.NotNull(result);
        //    Assert.Equal("Admin", result.ActionName);


        //    var deletedProduct = context.Set<Product>().FirstOrDefault(p => p.Id == addedProduct.Id);
        //    Assert.Null(deletedProduct);
        //}
    }
}
