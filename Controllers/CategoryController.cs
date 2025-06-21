using Products.Data;
using Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Categorys.Models;
using Microsoft.Extensions.Logging;

namespace Buoi2.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(AppDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

       [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description, Logo")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Kiểm tra trùng tên danh mục
                    if (_context.Categories.Any(c => c.Name == category.Name))
                    {
                        ModelState.AddModelError("Name", "Tên danh mục đã tồn tại");
                        return View(category);
                    }

                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Lỗi database: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "Không thể lưu danh mục. Vui lòng thử lại.");
            }

            return View(category);
        }
        // Xem chi tiết danh mục
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // Hiển thị form sửa danh mục
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // Xử lý khi submit form sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Category category)
        {
            _logger.LogInformation("Edit action called with category: {@Category}", category);

            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm category hiện tại
                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;

                    // Đánh dấu là đã sửa đổi
                    _context.Entry(existingCategory).State = EntityState.Modified;

                    // Lưu thay đổi
                    var result = await _context.SaveChangesAsync();
                    _logger.LogInformation("SaveChanges result: {Result}", result);

                    if (result > 0)
                    {
                        _logger.LogInformation("Category updated successfully. New values: Name={Name}, Description={Description}", 
                            existingCategory.Name, existingCategory.Description);
                        TempData["Success"] = "Cập nhật danh mục thành công!";
                        return Json(new { success = true, message = "Cập nhật thành công!" });
                    }
                    else
                    {
                        _logger.LogWarning("No changes were saved to the database");
                        return Json(new { success = false, message = "Không có thay đổi nào được lưu!" });
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating category");
                    if (!_context.Categories.Any(e => e.Id == category.Id))
                        return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating category");
                    return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật danh mục: " + ex.Message });
                }
            }
            else
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                _logger.LogWarning("Invalid model state: {@Errors}", errors);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
            }
        }

        // Hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // Xử lý xóa danh mục
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                try
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Category deleted successfully");
                    TempData["Success"] = "Xóa danh mục thành công!";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting category");
                    TempData["Error"] = "Có lỗi xảy ra khi xóa danh mục: " + ex.Message;
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}