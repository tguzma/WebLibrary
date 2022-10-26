using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Services;

namespace WebLibrary.Controllers
{
    [Route("Store")] //TODO this controller + FE for created API methods
    public class BookController : Controller
    {
        private readonly MongoDBBookService _bookService;

        public BookController(MongoDBBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("Index")]
        public async Task<ActionResult> Index() => View(await _bookService.GetAsync());

        [HttpGet("Details")]
        public ActionResult Details(Guid id)
        {
            return View();
        }

        [HttpGet("Edit")]
        public ActionResult Edit(Guid id)
        {
            return View();
        }


        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, Book book)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost("Delete")]
        public ActionResult Delete(Guid id)
        {
            return View();
        }
    }
}
