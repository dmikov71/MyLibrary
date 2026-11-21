using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;
using MyLibrary.Web.Models;

namespace MyLibrary.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GenresController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;
        public GenresController(IGenreRepository genreRepository, ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _genreRepository = genreRepository;
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            var query = _genreRepository.GetAllWithSubgenres();

            var genres = await query.ToListAsync();

            return View(genres);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name");

            return View(new GenreFormDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreFormDto form)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryID", "Name", form.CategoryID);
                return View(form);
            }

            try
            {
                // Конвертируем DTO в сущность домена
                var genre = new Genre
                {
                    Name = form.Name,
                    CategoryID = form.CategoryID.Value 
                };

                await _genreRepository.AddAsync(genre);
                TempData["Success"] = $"Жанр \"{genre.Name}\" успешно добавлен.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Не удалось сохранить: {ex.Message}");

                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryID", "Name", form.CategoryID);
                return View(form);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest("ID не указан");

            var genre = await _genreRepository.GetByIdAsync(id.Value);
            if (genre == null)
                return NotFound($"Жанр с ID {id} не найден");

            // Заполняем DTO данными сущности
            var form = new GenreFormDto
            {
                GenreID = genre.GenreID,
                Name = genre.Name,
                CategoryID = genre.CategoryID
            };

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name", genre.CategoryID);

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GenreFormDto form)
        {
            if (id != form.GenreID)
                return BadRequest("Несовпадение ID");

            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryID", "Name", form.CategoryID);
                return View(form);
            }

            try
            {
                var genre = await _genreRepository.GetByIdAsync(id);
                if (genre == null)
                    return NotFound($"Жанр с ID {id} не найден");

                // Обновляем поля
                genre.Name = form.Name;
                genre.CategoryID = form.CategoryID.Value;

                await _genreRepository.UpdateAsync(genre);
                TempData["Success"] = $"Жанр \"{genre.Name}\" успешно обновлён.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при сохранении: {ex.Message}");
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryID", "Name", form.CategoryID);
                return View(form);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Genre? genre = null;
            genre = await _genreRepository.GetByIdAsync(id);
            // Проверяем, есть ли книги, ссылающиеся на этот жанр
            var hasBooks = await _bookRepository.AnyAsync(b => b.GenreID == id);
            try
            {
                await _genreRepository.DeleteAsync(id);
                TempData["Success"] = $"Жанр \"{genre?.Name}\" успешно удалён. Связанные поджанры также удалены.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = $"Не удалось удалить жанр \"{genre?.Name}\": он связан с книгами в каталоге. Сначала удалите книги.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = $"Произошла непредвиденная ошибка при удалении";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
