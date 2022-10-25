using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Services;
using WebLibrary.Utils;

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
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet("Edit")]
        public async Task<ActionResult> Edit(Guid? id = null)
        {
            var user = await (id.HasValue ?  _userManager.FindByIdAsync(id.ToString()) : _userManager.GetUserAsync(_signInManager.Context.User));

            return View("AccountSettings", user);
        }

        [HttpGet("UserManagement")]
        public async Task<ActionResult> UserManagement()
        {
            if (IsLibrarian())
            {
                var role = await _roleManager.FindByNameAsync(Constants.Customer);
                var customers = await _userService.GetUsersByRoleAsync(role.Id);

                return View(customers);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Approve")]
        public async Task<ActionResult> Approve(Guid userId) 
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.IsApproved = true;
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpPost("Ban")]
        public async Task<ActionResult> Ban(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.IsBanned = !user.IsBanned;
            await _userManager.UpdateAsync(user);

            return Ok(user.IsBanned);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (IsLibrarian())
            {
                user.IsApproved = true;
                await RegisterCustomer(user);
                await _userManager.AddToRoleAsync(user, Constants.Customer);

            }
            else
            {
                var librarians = await _userManager.GetUsersInRoleAsync(Constants.Librarian);
                await RegisterCustomer(user);
                await _userManager.AddToRoleAsync(user, (librarians.Count > 0 ? Constants.Customer : Constants.Librarian));
                await _signInManager.SignInAsync(user, false);
                
                if (IsLibrarian())
                {
                    user.IsApproved = true;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!IsLibrarian())
            {
                user.IsApproved = false;
            }

            /*
             TODO:
               Editing
             */

            return IsLibrarian() ? RedirectToAction("UserManagement") : RedirectToAction("Index", "Home");
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

            return View("Login");
        }

        [HttpGet("Logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        private bool IsLibrarian() => _signInManager.Context.User.IsInRole(Constants.Librarian);
        private async Task RegisterCustomer(User user)
        {
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, user.PasswordHash);
            await _userManager.CreateAsync(user);
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
