using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Dtos;
using WebLibrary.Services;
using WebLibrary.Utils;
using static System.Net.WebRequestMethods;
using static System.Reflection.Metadata.BlobBuilder;

namespace WebLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly MongoDBBookService _bookService;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly MongoDBLoanService _loanService;

        public BookController(MongoDBBookService bookService, IMapper mapper,
            SignInManager<User> signInManager, UserManager<User> userManager,
            MongoDBLoanService loanService
            )
        {
            _bookService = bookService;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _loanService = loanService;

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

        [HttpGet("History")]
        public async Task<ActionResult> History(string id)
        {
            var user = await GetUserAsync(id);
            var books = await _bookService.GetAsync();

            var tuple = new Tuple<UserDto, List<BookDto>>(_mapper.Map(user, new UserDto()), _mapper.Map(books,new List<BookDto>()));
            
            return View(tuple);
        }
         
        [HttpGet("BorrowedBooks")]
        public async Task<ActionResult> BorrowedBooks(string id)
        {
            return View(_mapper.Map(await _bookService.GetAsync(), new List<BookDto>()));
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

            await _bookService.UpdatetAsync(book);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(string bookId)
        {
            if ((await _bookService.FindByIdAsync(bookId)).AmountBorrowed <= 0)
            {
                return Ok(); // this needs to send warning message "Book cannot be deleted"
            }
            
            await _bookService.DeleteAsync(bookId);

            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost("Borrow")]
        public async Task<ActionResult> Borrow(string bookId, Guid? userId)
        {
            var book = await _bookService.FindByIdAsync(bookId);
            var user = await GetUserAsync(userId.ToString());
            

            if (book.AmountAvalible <= 0 || user.BookIds.Contains(bookId) || user.BookIds.Count >= 6)
            {
                return base.Content("Book cannot be borrowed"); 
            }

            user.BookIds.Add(bookId);
            user.BookHistory.Add(new HistoryEntry(bookId, DateTime.Now, null));

            book.AmountAvalible--;
            book.AmountBorrowed++;

            var loan = new Loan();

            loan.BookId = bookId;
            loan.UserId = user.Id.ToString();
            loan.CreatedAt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            await _loanService.CreateAsync(loan);

            await _bookService.UpdatetAsync(book);
            await _userManager.UpdateAsync(user);

            return Json(book.AmountAvalible);
        }

        [HttpPost("Return")]
        public async Task<ActionResult> Return(string bookId, Guid? userId)
        {
            var book = await _bookService.FindByIdAsync(bookId);
            var user = await GetUserAsync(userId.ToString());

            if (!user.BookIds.Contains(bookId))
            {
                return base.Content("Book cannot be borrowed");
            }

            await _loanService.DeleteAsync(bookId, user.Id.ToString());
            user.BookIds.Remove(bookId);
            user.BookHistory.FirstOrDefault(x => x.BookId == bookId && !x.DateReturned.HasValue).DateReturned = DateTime.Now;
            book.AmountAvalible++;
            book.AmountBorrowed--;


            await _bookService.UpdatetAsync(book);
            await _userManager.UpdateAsync(user);

            return Json(book.AmountAvalible);
        }

        [HttpPost("BorrowOrReturn")]
        public async Task<ActionResult> BorrowOrReturn(string bookId, string username)
        {
            var book = await _bookService.FindByIdAsync(bookId);
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return base.Content("Book cannot be returned");
            }

            var loan = await _loanService.FindByIdAsync(bookId,user.Id.ToString());
            var wasBorrowed = !(loan.LoanId == null);

            if (wasBorrowed) //return
            {
                await _loanService.DeleteAsync(bookId, user.Id.ToString());
                user.BookIds.Remove(bookId);
                user.BookHistory.FirstOrDefault(x => x.BookId == bookId && !x.DateReturned.HasValue).DateReturned = DateTime.Now;
                book.AmountAvalible++;
                book.AmountBorrowed--;
            }
            else //borrow
            {
                if (book.AmountAvalible <= 0 || user.BookIds.Count >= 6)
                {
                    return Json(new { error = true});
                }

                user.BookIds.Add(bookId);
                user.BookHistory.Add(new HistoryEntry(bookId, DateTime.Now, null));

                book.AmountAvalible--;
                book.AmountBorrowed++;

                loan.BookId = bookId;
                loan.UserId = user.Id.ToString();
                loan.CreatedAt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                await _loanService.CreateAsync(loan);
            }

            await _bookService.UpdatetAsync(book);
            await _userManager.UpdateAsync(user);

            return Json(new {book.AmountAvalible, wasBorrowed });
        }

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

        private async Task<User> GetUserAsync(string id) => string.IsNullOrEmpty(id) ? await _userManager.GetUserAsync(_signInManager.Context.User) : await _userManager.FindByIdAsync(id);
    }
}
