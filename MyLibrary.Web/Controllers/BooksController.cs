using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Application.Helpers;
using MyLibrary.Application.Services;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Enums;
using MyLibrary.Domain.Repositories;
using MyLibrary.Web.Models;
using System.Data;

namespace MyLibrary.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubgenreRepository _subgenreRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IBookLibService _bookLibService;

        public BooksController(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IGenreRepository genreRepository,
            ICategoryRepository categoryRepository,
            ISubgenreRepository subgenreRepository,
            IPlaceRepository placeRepository,
            IBookLibService bookLibService)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
            _categoryRepository = categoryRepository;
            _subgenreRepository = subgenreRepository;
            _placeRepository = placeRepository;
            _bookLibService = bookLibService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            const int pageSize = 10;

            var booksQuery = await _bookRepository.GetAllWithDetailsAsync();

            if (!string.IsNullOrEmpty(search))
            {
                booksQuery = booksQuery.Where(b =>
                    b.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.BookAuthors.Any(ba =>
                        ba.Author.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var books = booksQuery.ToList();

            var viewModel = books
                .GroupBy(b => b.Category?.Code ?? "UNK")
                .SelectMany(g => g.Select((book, index) => new BookViewModel
                {
                    Id = book.BookID,
                    Name = book.Name,
                    LibraryNumber = book.LibraryNumber,
                    CategoryCode = book.Category?.Code ?? "UNK",
                    AuthorsDisplay = string.Join(", ",
                        book.BookAuthors.OrderBy(ba => ba.SortOrder)
                            .Select(ba => ba.Author.DisplayName)),
                    Volumes = book.Volumes,
                    BookTypeDisplay = BookDisplayHelper.GetBookTypeText(book.Type),
                    DisplayLibraryNumber = $"{book.LibraryNumber:D4}-{(index + 1):D4}{book.Category?.Code?.Trim().ToUpper()}"
                }))
                .OrderBy(b => b.LibraryNumber)
                .ToList();

            // --- пагинация ---
            var count = viewModel.Count;
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var items = viewModel
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentSearch = search;

            return View(new PaginatedList<BookViewModel>(items, count, page, pageSize));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetByIdWithDetailsAsync(id);
            if (book == null)
                return NotFound();

            var categoryCode = book.Category?.Code ?? "UNK";

            // Получаем порядковый номер книги внутри категории
            int categoryOrder = await _bookRepository.GetBookOrderInCategoryAsync(id);
            var displayNumber = $"{book.LibraryNumber:D4}-{categoryOrder:D4}{categoryCode.Trim().ToUpper()}";

            var viewModel = new BookViewModel
            {
                Id = book.BookID,
                Name = book.Name,
                LibraryNumber = book.LibraryNumber,
                DisplayLibraryNumber = displayNumber,
                CategoryCode = categoryCode,
                AuthorsDisplay = string.Join(", ",
                    book.BookAuthors.OrderBy(ba => ba.SortOrder)
                        .Select(ba => ba.Author.DisplayName)),
                CategoryName = book.Category?.Name,
                GenreName = book.Genre?.Name,
                SubgenreName = book.Subgenre?.Name,
                PlaceName = book.Place?.Name,
                Volumes = book.Volumes,
                BookTypeDisplay = BookDisplayHelper.GetBookTypeText(book.Type),
                SubtypeDisplay = BookDisplayHelper.GetSubtypeText(book.Subtype),
                ReadDisplay = ReadDisplayHelper.GetBookReadText(book.Read),
                Raiting = book.Raiting,
                ISBN = book.ISBN,
                Publishing = book.Publishing,
                Year = book.Year,
                Pages = book.Pages,
                CoverImage = book.CoverImage
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FillByIsbn(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            var result = await _bookLibService.FetchByIsbnAsync(book.ISBN);

            if (result == null)
            {
                TempData["Error"] = "Не удалось получить данные из Open Library. Проверьте ISBN или попробуйте позже.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Обновляем год, только если в БД 0
            if (book.Year == 0 && result.Year.HasValue)
                book.Year = result.Year.Value;

            // Обновляем страницы, только если в БД 0
            if (book.Pages == 0 && result.Pages.HasValue)
                book.Pages = result.Pages.Value;

            bool isCoverBroken = !string.IsNullOrEmpty(book.CoverImage) &&
                                 book.CoverImage.Contains("archive.org");

            // Обновляем обложку, если в базе пусто
            bool isOldLinkBroken = !string.IsNullOrEmpty(book.CoverImage) &&
                       book.CoverImage.Contains("archive.org");
            // Добавляем проверку на заглушку — чтобы перезаписать даже её
            bool isPlaceholder = book.CoverImage == "/img/no-cover.png";

            if ((string.IsNullOrEmpty(book.CoverImage) || isOldLinkBroken || isPlaceholder) &&
        !string.IsNullOrEmpty(result.CoverUrl))
            {
                book.CoverImage = result.CoverUrl;
            }

            if (string.IsNullOrEmpty(book.Publishing) && !string.IsNullOrEmpty(result.PublisherName))
                book.Publishing = result.PublisherName;

            await _bookRepository.UpdateAsync(book);

            TempData["Success"] = "Данные книги обновлены из Open Library.";
            return RedirectToAction(nameof(Details), new { id });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateViewBagForCreate();

            var model = new BookFormDto
            {
                Read = Read.NoRead,
                Volumes = 1
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormDto dto)
        {
            if (dto.SelectedAuthorIds == null || !dto.SelectedAuthorIds.Any())
            {
                ModelState.AddModelError(nameof(dto.SelectedAuthorIds), "Выберите хотя бы одного автора");
            }

            if (dto.Type == BookType.Electronic && !dto.Subtype.HasValue)
            {
                ModelState.AddModelError(nameof(dto.Subtype), "Для электронной книги обязательно укажите формат");
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewBagForCreate();
                return View(dto);
            }

            try
            {
                int nextSeq = await _bookRepository.GetTotalBookCountAsync() + 1;

                var book = new Book
                {
                    Name = dto.Name,
                    LibraryNumber = nextSeq,
                    CategoryID = dto.CategoryID.Value,
                    GenreID = dto.GenreID.Value,
                    SubgenreID = dto.SubgenreID,
                    PlaceID = dto.PlaceID.Value,
                    Volumes = dto.Volumes,
                    Type = dto.Type,
                    Subtype = dto.Subtype,
                    Read = dto.Read,
                    Raiting = dto.Raiting,
                    ISBN = dto.ISBN
                };

                // Авторы: порядок выбора = SortOrder
                int order = 1;
                foreach (var authorId in dto.SelectedAuthorIds.Distinct())
                {
                    book.BookAuthors.Add(new BookAuthor
                    {
                        AuthorID = authorId,
                        SortOrder = order++
                    });
                }

                await _bookRepository.AddAsync(book);
                TempData["Success"] = $"Книга \"{book.Name}\" успешно добавлена.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Не удалось сохранить книгу: {ex.Message}");
                await PopulateViewBagForCreate();
                return View(dto);
            }
        }

        [Authorize(Roles = "Admin")]
        private async Task PopulateViewBagForCreate()
        {
            ViewBag.Authors = new SelectList(
                await _authorRepository.GetSortedByNameQueryable().ToListAsync(),
                "AuthorID", "DisplayName");  

            ViewBag.Categories = new SelectList(
                await _categoryRepository.GetAllAsync(),
                "CategoryID", "Name");

            ViewBag.GenresList = await _genreRepository.GetAllAsync();

            ViewBag.SubgenresList = (await _subgenreRepository.GetAllAsync()).Select(s => new
            {
                id = s.SubgenreID,
                name = s.Name,
                genreId = s.GenreID
            }).ToList();

            ViewBag.Places = new SelectList(
                await _placeRepository.GetAllAsync(),
                "PlaceID", "Name");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetByIdWithDetailsAsync(id);
            if (book == null)
                return NotFound();

            var dto = new BookFormDto
            {
                BookID = book.BookID,
                Name = book.Name,
                SelectedAuthorIds = book.BookAuthors
                    .OrderBy(ba => ba.SortOrder)
                    .Select(ba => ba.AuthorID)
                    .ToList(),
                CategoryID = book.CategoryID,
                GenreID = book.GenreID,
                SubgenreID = book.SubgenreID,
                PlaceID = book.PlaceID,
                Volumes = book.Volumes,
                Type = book.Type,
                Subtype = book.Subtype,
                Read = book.Read,
                Raiting = book.Raiting,
                ISBN = book.ISBN
            };

            await PopulateViewBagForCreate();
            return View(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormDto dto)
        {
            if (dto.SelectedAuthorIds == null || !dto.SelectedAuthorIds.Any())
            {
                ModelState.AddModelError(nameof(dto.SelectedAuthorIds), "Выберите хотя бы одного автора");
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewBagForCreate();
                return View(dto);
            }

            try
            {
                var book = await _bookRepository.GetByIdAsync(dto.BookID);
                if (book == null)
                    return NotFound();

                book.Name = dto.Name;
                book.CategoryID = dto.CategoryID.Value;
                book.GenreID = dto.GenreID.Value;
                book.SubgenreID = dto.SubgenreID;
                book.PlaceID = dto.PlaceID.Value;
                book.Volumes = dto.Volumes;
                book.Type = dto.Type;
                book.Subtype = dto.Subtype;
                book.Read = dto.Read;
                book.Raiting = dto.Raiting;
                book.ISBN = dto.ISBN;

                await _bookRepository.RemoveAllBookAuthorsAsync(book.BookID);

                int order = 1;
                foreach (var authorId in dto.SelectedAuthorIds.Distinct())
                {
                    await _bookRepository.AddBookAuthorAsync(book.BookID, authorId, order++);
                }

                await _bookRepository.SaveChangesAsync(); 

                TempData["Success"] = $"Книга \"{book.Name}\" успешно обновлена.";
                return RedirectToAction(nameof(Details), new { id = book.BookID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Не удалось сохранить изменения: {ex.Message}");
                await PopulateViewBagForCreate();
                return View(dto);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Book? book = null;
            book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            try
            {
                await _bookRepository.DeleteAsync(id);
                TempData["Success"] = $"Книга \"{book.Name}\" успешно удалена.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Message.Contains("REFERENCE"))
                {
                    TempData["Error"] = $"Не удалось удалить книгу \"{book.Name}\": у неё есть связанные записи (например, авторы).";
                }
                else
                {
                    TempData["Error"] = $"Не удалось удалить книгу \"{book.Name}\": {ex.Message}";
                }
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
