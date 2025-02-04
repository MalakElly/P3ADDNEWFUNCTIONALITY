using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Resources.Models.Services;
using ProductService = P3AddNewFunctionalityDotNetCore.Models.Services.ProductService;



namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        public class ProductViewModelValidationTest
        {
            private readonly Mock<Models.Services.ProductService> mockProductService;
            private readonly ProductViewModel product;
            Models.Services.ProductService productService = new Models.Services.ProductService(It.IsAny<ICart>(), It.IsAny<IProductRepository>(), It.IsAny<IOrderRepository>(), It.IsAny<IStringLocalizer<Models.Services.ProductService>>());


            public ProductViewModelValidationTest()
            {
                var mockProductService = new Mock<Models.Services.ProductService>();
                this.mockProductService = mockProductService;
                product = new ProductViewModel();


            }

            [Fact]
            public void TestMissingName()
            {
                product.Name = null;
                product.Price = "10.0";
                product.Stock = "1";
                var result = ValidModel(product);
                Assert.True(result.Any());
                Assert.Equal(result.FirstOrDefault(), "Veuillez saisir un nom");
            }
           
            [Fact]
            public void TestMissingPrice()
            {
                product.Name = "prod1";
                product.Price = null;
                product.Stock = "1";
                var result = ValidModel(product);
                Assert.True(result.Any());
                Assert.Equal(result.FirstOrDefault(), "Veuillez saisir un prix");
            }
            
            [Fact]
            public void TestMissingStock()
            {
                product.Name = "prod1";
                product.Price = "10.0";
                product.Stock = null;
                var result = ValidModel(product);
                Assert.True(result.Any());
                Assert.Equal(result.FirstOrDefault(), "Veuillez saisir un stock");
            }

            
            [Fact]
            public void TestPriceNotANumber()
            {
                product.Name = "Test Product";
                product.Price = "j"; // Not a Number
                product.Stock = "3";
                var result = ValidModel(product);
                Assert.True(result.Any());
                Assert.Equal(result.FirstOrDefault(), "La valeur saisie pour le prix doit être un nombre");
            }

            [Fact]
            public void TestPriceNotGreaterThanZero()
            {
                product.Name = "Test Product";
                product.Price = "0";
                product.Stock = "1";
                var result = ValidModel(product);
                Assert.True(result.Any());
                Assert.Equal(result.FirstOrDefault(), "Le prix doit être supérieur à zéro");
            }
            [Fact]
            public void TestQuantityNotANumber()
            {
                product.Name = "Test Product";
                product.Price = "10.0";
                product.Stock = "5.5";
                var result = ValidModel(product);
                Assert.True(result.Any());
                Assert.Equal(result.FirstOrDefault(), "La valeur saisie pour le stock doit être un entier");
            }
         
            [Fact]
            public void TestQuantityNotGreaterThanZero()
            {
                product.Name = "Test Product";
                product.Price = "10.0";
                product.Stock = "0";
                var result = ValidModel(product);
                Assert.True(result.Any());
                Assert.Equal(result.FirstOrDefault(), "Le stock doit être supérieure à zéro");
            }



            private List<string> ValidModel(ProductViewModel model)
            {
                return productService.ValidateModel(model);
            }

        }
    }
}

