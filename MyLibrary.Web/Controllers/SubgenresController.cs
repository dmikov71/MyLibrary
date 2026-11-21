using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;

namespace MyLibrary.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubgenresController : Controller
    {
        private readonly ISubgenreRepository _subgenreRepository;
        private readonly IBookRepository _bookRepository;
        public SubgenresController(ISubgenreRepository subgenreRepository, IBookRepository bookRepository)
        {
            _subgenreRepository = subgenreRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            var subgenres = await _subgenreRepository.GetAllAsync();
            return View(subgenres);
        }
        [HttpGet]
        public IActionResult Create(int? genreId)
        {
            var model = new Subgenre();
            if (genreId.HasValue)
            {
                model.GenreID = genreId.Value;
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subgenre subgenre)
        {
            if (!ModelState.IsValid)
            {
                // Ошибки валидации показываем прямо в форме 
                return View(subgenre);
            }

            try
            {
                await _subgenreRepository.AddAsync(subgenre);
                TempData["Success"] = $"Поджанр \"{subgenre.Name}\" успешно добавлен.";
                // Возвращаемся на страницу жанров
                return RedirectToAction("Index", "Genres");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Не удалось создать поджанр: нарушено ограничение внешнего ключа (проверьте, что жанр существует).";
                return View(subgenre);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Произошла ошибка при создании поджанра: {ex.Message}";
                return View(subgenre);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var subgenre = await _subgenreRepository.GetByIdAsync(id.Value);
            if (subgenre == null)
                return NotFound();

            return View(subgenre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subgenre subgenre)
        {
            if (id != subgenre.SubgenreID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(subgenre);
            }

            try
            {
                await _subgenreRepository.UpdateAsync(subgenre);
                TempData["Success"] = $"Поджанр \"{subgenre.Name}\" успешно обновлён.";
                return RedirectToAction("Index", "Genres");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Не удалось сохранить изменения: {ex.Message}";
                return RedirectToAction("Index", "Genres");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Subgenre? subgenre = null;
            // Проверяем, есть ли книги, ссылающиеся на этот поджанр
            var hasBooks = await _bookRepository.AnyAsync(b => b.SubgenreID == id);
            try
            {
                subgenre = await _subgenreRepository.GetByIdAsync(id);

                if (subgenre == null)
                    return NotFound();

                await _subgenreRepository.DeleteAsync(id);

                TempData["Success"] = $"Поджанр \"{subgenre.Name}\" успешно удалён.";

                return RedirectToAction("Index", "Genres");
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = $"Не удалось удалить поджанр \"{subgenre?.Name ?? "неизвестный"}\": с ним связаны книги в каталоге.";

                return RedirectToAction("Index", "Genres");
            }
            catch (Exception)
            {
                TempData["Error"] = "Произошла непредвиденная ошибка при удалении.";
                return RedirectToAction("Index", "Genres");
            }
        }

    }
}
