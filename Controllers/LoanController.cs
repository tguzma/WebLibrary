using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebLibrary.Models;
using WebLibrary.Services;

namespace WebLibrary.Controllers
{
    public class LoanController : Controller
    {
        private readonly MongoDBLoanService _loanService;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public LoanController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
    }
}
