using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Dtos;
using WebLibrary.Services;

namespace WebLibrary.Controllers
{
    public class ExportImportController : Controller
    {
        private readonly MongoDBBookService _dbService;
        private readonly SignInManager<User> _signInManager;


        public ExportImportController(MongoDBBookService dbService, SignInManager<User> signInManager)
        {
            _dbService = dbService;
            _signInManager = signInManager;

        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction(nameof(Index), "Book");
            }

            return View();
        }

        [HttpGet("Export")]
        public ActionResult Export()
        {
            File("c://test.json", "application/json", "json.json");
            return RedirectToAction(nameof(Index), "Book");
        }
    }
}
