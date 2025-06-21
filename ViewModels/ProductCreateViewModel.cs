using Microsoft.AspNetCore.Http;
using Products.Models;

namespace Products.ViewModels
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; } = new Product();
        public IFormFile? Image { get; set; }
    }
} 