using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;

namespace MyLibrary.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;
        public CategoriesController(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }
        public async Task <IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                TempData["Success"] = $"Категория \"{category.Name}\" успешно добавлена.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var category = await _categoryRepository.GetByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                await _categoryRepository.UpdateAsync(category);
                TempData["Success"] = $"Категория \"{category.Name}\" успешно обновлена.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Не удалось сохранить изменения: {ex.Message}";
                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = null;

                category = await _categoryRepository.GetByIdAsync(id);

                if (category == null)
                    return NotFound();

            // Проверяем, есть ли книги, ссылающиеся на эту категорию
            var hasBooks = await _bookRepository.AnyAsync(b => b.CategoryID == id);

            try
            {
                await _categoryRepository.DeleteAsync(id);
                TempData["Success"] = $"Категория \"{category.Name}\" успешно удалена.";
            }
            catch (DbUpdateException) 
            {
                TempData["Error"] = $"Не удалось удалить категорию \"{category.Name}\": она связана с книгами в каталоге. Сначала удалите книги.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Произошла непредвиденная ошибка при удалении.";
            }

            return RedirectToAction(nameof(Index));
            
        }
    }
}
