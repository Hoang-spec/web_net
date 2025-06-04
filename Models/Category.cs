using System;
using Products.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Categorys.Models
{
public class Category
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    public string Description { get; set; }
    
    public string Logo { get; set; }
    
    // Navigation property
        public virtual ICollection<Products.Models.Product>? Products { get; set; }
}
}