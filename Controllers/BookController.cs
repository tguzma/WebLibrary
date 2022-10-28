using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Dtos;
using WebLibrary.Services;
using WebLibrary.Utils;

namespace WebLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly MongoDBBookService _bookService;
        private readonly IMapper _mapper;

        public BookController(MongoDBBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet("Index")]
        public async Task<ActionResult> Index()
        {
            return View(_mapper.Map(await _bookService.GetAsync(), new List<BookDto>()));
        }

        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpGet("Edit")]
        public async Task<ActionResult> Edit(string id)
        {
            var book = await _bookService.FindByIdAsync(id);

            return View(_mapper.Map(book,new BookDto()));
        }    

        [HttpGet("Detail")]
        public async Task<ActionResult> Detail(string id)
        {
            var book = await _bookService.FindByIdAsync(id);

            return View(_mapper.Map(book, new BookDto()));
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error","Home");
            }

            var book = _mapper.Map(bookDto, new Book());
            book.ImageUrl = await SaveImageAsync(bookDto.Image);

            await _bookService.CreateAsync(book);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string bookId, BookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            var book = await _bookService.FindByIdAsync(bookId);
      
            if (bookDto.Image != null)
            {
                DeleteImage(book.ImageUrl);
                book.ImageUrl = await SaveImageAsync(bookDto.Image);
            }

            bookDto.ImageUrl = book.ImageUrl;
            _mapper.Map(bookDto, book);

            await _bookService.UpdatetAsync(bookId, book);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(string bookId)
        {
            /*add logic that denies deletion when any user has book*/
            await _bookService.DeleteAsync(bookId);

            return RedirectToAction(nameof(Index));
        }

        /* if needed in future for smth, can be just deleted tbh 
        private FormFile GetImage(string imageUrl)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(Path.GetFileName(imageUrl), out string contentType);

            using (var stream = System.IO.File.OpenRead(Path.GetFullPath(imageUrl)))
            {
                return new FormFile(stream, 0, stream.Length, null, Path.GetFileName(imageUrl))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType
                };
            }
        }
        */

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var imageUrl = Path.Combine(Constants.FilePath, Guid.NewGuid() + Path.GetExtension(image.FileName));
            using (var fileStream = new FileStream(Path.GetFullPath(imageUrl), FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return imageUrl;
        }

        private void DeleteImage(string path)
        {
            var fullPath = Path.GetFullPath(path);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
