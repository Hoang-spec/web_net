using Products.Data;
using Products.Models;
using Products.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Categorys.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Buoi2.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(AppDbContext context, ILogger<ProductController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Detailss(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View("DetailsP",product);
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

        // Hiển thị form thêm mới
        public IActionResult Create()
        {
            try
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
                return View(new ProductCreateViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories for create form");
                TempData["Error"] = "Không thể tải danh sách danh mục: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý khi submit form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            _logger.LogInformation("Create action called with product: {@Product}", viewModel.Product);

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.Image != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Image.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.Image.CopyToAsync(fileStream);
                        }

                        viewModel.Product.Image = "/images/products/" + uniqueFileName;
                    }

                    _context.Add(viewModel.Product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product created successfully");
                    TempData["Success"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating product");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu sản phẩm: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state: {@ModelState}", ModelState);
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                }
            }

            // Nếu có lỗi, load lại danh sách category
            try
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", viewModel.Product.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories after failed create");
                ModelState.AddModelError("", "Không thể tải danh sách danh mục: " + ex.Message);
            }
            
            return View(viewModel);
        }

        // Xem chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // Hiển thị form sửa sản phẩm
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý khi submit form sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,CategoryId,image")] Product product)
        {
            if (id != product.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id))
                        return NotFound();
                    else throw;
                }
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}