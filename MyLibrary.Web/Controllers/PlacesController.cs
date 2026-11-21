using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;

namespace MyLibrary.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlacesController : Controller
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IBookRepository _bookRepository;
        public PlacesController(IPlaceRepository placeRepository, IBookRepository bookRepository)
        {
            _placeRepository = placeRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
           var places = await _placeRepository.GetAllAsync();
            return View(places);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Place place)
        {
            if (ModelState.IsValid)
            {
                await _placeRepository.AddAsync(place);
                TempData["Success"] = $"Место хранения \"{place.Name}\" успешно добавлено.";
                return RedirectToAction(nameof(Index));
            }
            return View(place);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var author = await _placeRepository.GetByIdAsync(id.Value);
            if (author == null)
                return NotFound();

            return View(author);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Place place)
        {
            if (id != place.PlaceID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(place);
            }

            try
            {
                await _placeRepository.UpdateAsync(place);
                TempData["Success"] = $"Место хранения \"{place.Name}\" успешно обновлено.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Не удалось сохранить изменения: {ex.Message}";
                return View(place);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Place? place = null;

                place = await _placeRepository.GetByIdAsync(id);

                if (place == null)
                    return NotFound();

            // Проверяем, есть ли книги, ссылающиеся на это место
            var hasBooks = await _bookRepository.AnyAsync(b => b.PlaceID == id);

            try
            {
                await _placeRepository.DeleteAsync(id);
                TempData["Success"] = $"Место хранения\"{place.Name}\" успешно удалено.";
            }
            catch (DbUpdateException) 
            {
                TempData["Error"] = $"Не удалось удалить место хранения \"{place.Name}\": оно связано с книгами в каталоге. Сначала удалите книги.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Произошла непредвиденная ошибка при удалении.";
            }
            return RedirectToAction(nameof(Index));
           
        }
    }
}
