using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }


        [Required(ErrorMessageResourceName = "MissingName", ErrorMessageResourceType = typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService))]
        public string Name { get; set; }


        
        public string Description { get; set; }


        
        public string Details { get; set; }

        [Required(ErrorMessageResourceName = "MissingStock", ErrorMessageResourceType = typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService))]
        [RegularExpression(@"^\s*\d+\s*$", ErrorMessageResourceName = "StockNotAnInteger", ErrorMessageResourceType = typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "StockNotGreaterThanZero", ErrorMessageResourceType = typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService))]
        public string Stock { get; set; }

        [Required(ErrorMessageResourceName = "MissingPrice", ErrorMessageResourceType = typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService))]
        [RegularExpression(@"^\s*\d+(\.\d+)?\s*$", ErrorMessageResourceName = "PriceNotANumber", ErrorMessageResourceType = typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService))]
        [Range(1, double.MaxValue, ErrorMessageResourceName = "PriceNotGreaterThanZero", ErrorMessageResourceType = typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService))]
        public string Price { get; set; }

    }
}
