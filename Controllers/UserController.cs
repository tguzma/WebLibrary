
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Services;

namespace WebLibrary.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MongoDBUserService _userService;

        public UserController
            (SignInManager<User> signInManager, 
            UserManager<User> userManager, 
            MongoDBUserService userService,
            RoleManager<Role> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
        }

        [HttpGet("Register")]
        public ActionResult RegisterIndex()
        {
            return View();
        }

        [HttpGet("Login")]
        public ActionResult LoginIndex()
        {
            return View();
        }

        [HttpGet("ApproveAccounts")]
        public async Task <ActionResult> ApproveAccountIndex()
        {
            var user = _signInManager.Context.User;

            if (user.IsInRole("Librarian"))
            {
                var role = await _roleManager.FindByNameAsync("Customer");
                var customers = await _userService.GetUsersByRoleAsync(role.Id);

                return View(customers.Where(x => !x.EmailConfirmed));
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Approve")]
        public async Task<ActionResult> ApproveAccount(Guid userId) //TODO make this work
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("ApproveAccountIndex");
        }
        // POST: UserController/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var librarians = await _userManager.GetUsersInRoleAsync("Librarian");

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, user.PasswordHash);
            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, (librarians.Count > 0 ? "Customer" : "Librarian"));
            await _signInManager.SignInAsync(user,false);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string userName, string passwordHash)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if ((await _signInManager.PasswordSignInAsync(userName, passwordHash, false, false)).Succeeded)
            {
                return RedirectToAction("Index","Home");
            }

            return View("LoginIndex");
        }

        [HttpGet("Logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        /*
        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
        */
    }
}
