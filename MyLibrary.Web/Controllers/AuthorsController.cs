using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;
using MyLibrary.Web.Models;

namespace MyLibrary.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;

            var query = _authorRepository.GetSortedByNameQueryable();

            var count = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var authors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(new PaginatedList<Author>(authors, count, page, pageSize));
        }


        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                await _authorRepository.AddAsync(author);
                TempData["Success"] = $"Автор \"{author.DisplayName}\" успешно добавлен.";
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var author = await _authorRepository.GetByIdAsync(id.Value);
            if (author == null)
                return NotFound();

            return View(author);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.AuthorID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(author);
            }

            try
            {
                await _authorRepository.UpdateAsync(author);
                TempData["Success"] = $"Автор \"{author.DisplayName}\" успешно обновлён.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Не удалось сохранить изменения: {ex.Message}";
                return View(author);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
                return NotFound();

            var hasBooks = await _authorRepository.HasBooksAsync(id);

            if (hasBooks)
            {
                TempData["Error"] = $"Не удалось удалить автора \"{author.DisplayName}\": он связан с книгами в каталоге. Сначала удалите книги.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _authorRepository.DeleteAsync(id);
                TempData["Success"] = $"Автор \"{author.DisplayName}\" успешно удалён.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = $"Не удалось удалить автора \"{author.DisplayName}\".";
            }
            catch (Exception)
            {
                TempData["Error"] = "Произошла непредвиденная ошибка при удалении.";
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
