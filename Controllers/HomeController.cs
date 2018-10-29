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

        // To access the functionality below, you must be logged in.
        [Route("/dashboard")]
        [ServiceFilter(typeof(LoggedInAttribute))]
        public IActionResult Dashboard()
        {
            ViewData["Message"] = "Your success page.";
            ViewData["User"] = HttpContext.Session.GetInt32("User");
            List<Wedding> wed = _dbContext.Weddings
                .Include(wg=>wg.Creator)
                .Include(wg => wg.WeddingGuests)
                .ThenInclude(g=>g.User)
                .ToList();

            return View(wed);
        }

        [HttpGet]
        [Route("/logout")]
        [ServiceFilter(typeof(LoggedInAttribute))]
        public IActionResult Logout(){
            HttpContext.Session.Remove("User");
            return Redirect("/login");
        }

        [HttpGet]
        [Route("/delete/{weddingid}")]
        [ServiceFilter(typeof(LoggedInAttribute))]
        public IActionResult Create(int weddingid){
            int userid = (int)HttpContext.Session.GetInt32("User");
            Wedding wedding = _dbContext.Weddings
                .Include(c=>c.Creator)
                .Where(w=>w.WeddingId == weddingid).FirstOrDefault();

            if(wedding.Creator.UserId == userid) {
                _dbContext.Remove(wedding);
                _dbContext.SaveChanges();
            }
            
            return Redirect("/dashboard");
        }

        [HttpGet]
        [Route("/create")]
        [ServiceFilter(typeof(LoggedInAttribute))]
        public IActionResult Create(){
            return View();
        }

        [HttpPost]
        [Route("/create")]
        [ServiceFilter(typeof(LoggedInAttribute))]
        public IActionResult Create(Wedding model){
            if(!ModelState.IsValid) 
                return View();

            model.CreatorId = (int)HttpContext.Session.GetInt32("User");

            _dbContext.Add(model);
            _dbContext.SaveChanges();

            return Redirect("/dashboard");
        }

        [HttpGet]
        [Route("/rsvp/{weddingid}")]
        [ServiceFilter(typeof(LoggedInAttribute))]
        public IActionResult Rsvp(int weddingid){
            int userid = (int)HttpContext.Session.GetInt32("User");
            User user = _dbContext.Users.Where(u=>u.UserId == userid).First();
            Wedding wedding = _dbContext.Weddings
                .Include(w=>w.Creator).Where(w=>w.WeddingId== weddingid && w.CreatorId != userid).First();

            bool userWedding = _dbContext.UserWedding_Xrf.Any(uw=>uw.UserId == userid && uw.WeddingId == weddingid);
            
            if(userWedding == true) {
                UserWedding_Xrf newUserWedding = _dbContext.UserWedding_Xrf
                    .Where(uw=>uw.UserId == userid && uw.WeddingId == weddingid)
                    .First();
                
                _dbContext.UserWedding_Xrf.Remove(newUserWedding);
            } else if(userWedding == false){
                UserWedding_Xrf newUserWedding = new UserWedding_Xrf();
                newUserWedding.UserId = userid;
                newUserWedding.WeddingId = weddingid;
                newUserWedding.State = "RSPV";
                UserWedding_Xrf addedUserWedding = _dbContext.Add(newUserWedding).Entity;
                user.WeddingGuests.Add(addedUserWedding);
                wedding.WeddingGuests.Add(addedUserWedding);
            }

            _dbContext.SaveChanges();

            return Redirect("/dashboard");
        }

        [HttpGet]
        [Route("/guestlist/{weddingid}")]
        public IActionResult GuestList(int weddingid) {
            Wedding wedding = _dbContext.Weddings
            .Include(w=>w.Creator)
            .Include(w=>w.WeddingGuests)
            .ThenInclude(wg=>wg.User)
            .Where(w=>w.WeddingId == weddingid)
            .FirstOrDefault();

            ViewData["Heading"] = wedding.Wedder1 + " & " + wedding.Wedder2 + "'s Wedding";
            return View(wedding);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
