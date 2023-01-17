using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Dtos;
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
        private readonly IMapper _mapper;

        public UserController
            (SignInManager<User> signInManager, 
            UserManager<User> userManager, 
            MongoDBUserService userService,
            RoleManager<Role> roleManager,
            IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
            _mapper = mapper;
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

            if (!user.IsApproved && !IsLibrarian())
            {
                return RedirectToAction("Index", "Book");
            }

            return View("AccountSettings", _mapper.Map(user, new UserDto()));
        }

        [HttpGet("UserManagement")]
        public async Task<ActionResult> UserManagement()
        {
            if (IsLibrarian())
            {
                var role = await _roleManager.FindByNameAsync(Constants.Customer);
                var customers = await _userService.GetUsersByRoleAsync(role.Id);

                return View(_mapper.Map(customers, new List<UserDto>()));
            }

            return RedirectToAction("Index", "Book");
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
        public async Task<ActionResult> Create(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _mapper.Map(userDto,new User());

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

            return RedirectToAction("Index", "Book");
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            userDto.IsApproved = user.IsApproved;
            userDto.IsBanned = user.IsBanned;
            _mapper.Map(userDto, user);
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, user.PasswordHash);

            if (!IsLibrarian())
            {
                user.IsApproved = false;
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Book");
        }

        [HttpGet("Search")]
        public async Task<ActionResult> Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return RedirectToAction(nameof(UserManagement));
            }

            var result = await _userService.Search(term);

            return View("UserManagement", _mapper.Map(result, new List<UserDto>()));
        }

        [HttpGet("Autocomplete")]
        public async Task<ActionResult> Autocomplete()
        {
            var tags = await _userService.Autocomplete();

            return Json(new { tags });
        }

        [HttpGet("Sort")]
        public async Task<ActionResult> Sort(string sortType)
        {
            if (string.IsNullOrEmpty(sortType))
            {
                return RedirectToAction(nameof(UserManagement));
            }

            var result = await _userService.Sort(sortType);

            return View("UserManagement", _mapper.Map(result, new List<UserDto>()));
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
                return RedirectToAction("Index", "Book");
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
    }
}
