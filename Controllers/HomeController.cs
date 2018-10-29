using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wedding_Planner.Models;
using Wedding_Planner.Persistance;
using Wedding_Planner.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Wedding_Planner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingPlannerDbContext _dbContext;
        public HomeController(WeddingPlannerDbContext context) {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            // Check initial ModelState
            if(ModelState.IsValid)
            {
                if(_dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View();
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                User returnedUser = _dbContext.Add(user).Entity;
                _dbContext.SaveChanges();
                
                HttpContext.Session.SetInt32("User", returnedUser.UserId);
                return Redirect("/dashboard");
            }

            return View();
        } 

        [HttpGet]
        [Route("/login")]
        public IActionResult Login(){
            return View("Index");
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb == null || userSubmission.Password == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("Index");
                }
                
                HttpContext.Session.SetInt32("User", userInDb.UserId);
                return Redirect("/dashboard");
            }

            return View("Index");
        }

        [Route("/dashboard")]
        [ServiceFilter(typeof(LoggedInAttribute))]
        public IActionResult Dashboard()
        {
                ViewData["Message"] = "Your success page.";
                List<Wedding> wed = _dbContext.Weddings
                    .Include(wg => wg.WeddingGuests)
                    .ThenInclude(g=>g.User)
                    .ToList();

                return View(wed);
        }

        [HttpGet]
        public IActionResult Logout(){
                HttpContext.Session.SetString("User", null);
                return Redirect("/login");
        }

        [HttpGet]
        [Route("/create")]
        public IActionResult Create(){
            return View();
        }

        [HttpPost]
        [Route("/create")]
        public IActionResult Create(Wedding model){
            if(!ModelState.IsValid) 
                return View();

            _dbContext.Add(model);
            _dbContext.SaveChanges();

            return Redirect("/dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
