
using Library_Management_System.DataConnection;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Library_Management_System.Controllers
{
    public class HomeController : Controller
    {
       private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpPost]
        public JsonResult GetStates(int Country_Id)
        {
            var states = _context.State
                .Where(s => s.c_Id == Country_Id)
                .Select(s => new
                {
                    s.State_Id,
                    s.StateName
                }).ToList();

            return Json(states);
        }
        [HttpPost]
        public JsonResult GetCities(int StateId)
        {
            var states = _context.City
                .Where(s => s.s_Id == StateId)
                .Select(s => new
                {
                    s.City_Id,
                    s.CityName
                }).ToList();

            return Json(states);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            var model = new UserViewModel();
            model.CountryList = _context.Country.
                Select(c => new SelectListItem
                {
                    Value = c.Country_Id.ToString(),
                    Text = c.CountryName
                }).ToList();

            model.RoleList = _context.Role.
                Select(r=>new SelectListItem
            {
                Value=r.RoleId.ToString(),
                Text=r.RoleName
            }).ToList();
            return View(model);
        }
        [HttpPost]
        public IActionResult AddUser(UserViewModel uvm)
        {
            if (uvm.Users.Id == 0)
            {
                uvm.Users.CreatedDate = DateTime.Now;
                uvm.Users.IsDeleted = 0;
                uvm.Users.IsActive = true;
                _context.TblUser.Add(uvm.Users);
                _context.SaveChanges();

                uvm.Login = new Login
                {
                    EmailId = uvm.Users.Email,
                    RoleId = uvm.Users.RoleId.ToString(),
                    Password = uvm.Users.Password,
                    LoginId = uvm.Users.Id,
                    IsActive = uvm.Users.IsActive
                };
                _context.TblLogin.Add(uvm.Login);
                _context.SaveChanges();
            }
            else
            {
                    uvm.Users.IsActive = true;
                    uvm.Users.UpdatedDate = DateTime.Now;
                    _context.TblUser.Update(uvm.Users);
                    _context.SaveChanges();
                var login = _context.TblLogin.FirstOrDefault(l => l.LoginId == uvm.Users.Id);

                if (login != null)
                {
                    // Update existing login
                    login.EmailId = uvm.Users.Email;
                    login.RoleId = uvm.Users.RoleId.ToString();
                    login.Password = uvm.Users.Password;
                    login.IsActive = uvm.Users.IsActive;

                    _context.TblLogin.Update(login);
                }
                else
                {
                    login = new Login
                    {
                        EmailId = uvm.Users.Email,
                        RoleId = uvm.Users.RoleId.ToString(),
                        Password = uvm.Users.Password,
                        LoginId = uvm.Users.Id,
                        IsActive = uvm.Users.IsActive
                    };
                    _context.TblLogin.Add(login);
                }

                _context.SaveChanges();

            }
            return RedirectToAction("AddUser");
        }
        [HttpGet]
        public IActionResult UserList()
        {
            //var users = _context.TblUser.Where(from u in _context.TblUser
            //                                   join c in _context.Country on u.Country_Id equals c.Country_Id
            //                                   where u.IsDeleted == 0
            //                                   select new UserViewModel
            //                                   {
            //                                       CountryList = u.Country_Id(),

            //                                   }).ToList();
            var users=_context.TblUser.Where(u=>u.IsDeleted==0).ToList();
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var user=_context.TblUser.FirstOrDefault(u=>u.Id == Id);
            var model = new UserViewModel();
            model.CountryList = _context.Country.
                Select(c => new SelectListItem
                {
                    Value = c.Country_Id.ToString(),
                    Text = c.CountryName
                }).ToList();

            model.StateList=_context.State
                .Where(s=>s.c_Id==user.Country_Id)
                .Select(s => new SelectListItem
                {
                   Value=s.State_Id.ToString(),
                   Text=s.StateName
                })
                .ToList();

            model.CityList = _context.City
                .Where(s => s.s_Id == user.State_Id)
                .Select(s => new SelectListItem
                {
                    Value=s.City_Id.ToString(),
                    Text=s.CityName
                }).ToList();

            model.RoleList = _context.Role.
                Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.RoleName
                }).ToList();
            if (user == null)
            {
                return NotFound();
            }
            var viewmodel = new UserViewModel
            {
               Users=user,
               UserList=_context.TblUser.ToList(),
               CountryList=model.CountryList,
               RoleList=model.RoleList,
               StateList=model.StateList,
               CityList=model.CityList
               
            };
            return View("AddUser",viewmodel);
        }
        public IActionResult Delete(int Id)
        {
            var user = _context.TblUser.FirstOrDefault(u=>u.Id==Id);
            var u = _context.TblLogin.FirstOrDefault(u => u.LoginId == Id);
            if (user != null)
            {
                u.IsActive = false;
                user.IsDeleted = 1;
                user.IsActive = false;
                _context.SaveChanges();
            }
            return RedirectToAction("UserList");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string emailid,string password)
        {

            var user=_context.TblLogin.FirstOrDefault(u=>u.EmailId==emailid && u.Password==password);
            if (user != null)
            {
				HttpContext.Session.SetString("UserId", user.EmailId);
				HttpContext.Session.SetString("Password", user.Password);
				HttpContext.Session.SetString("LoginId", user.Id.ToString());

				return RedirectToAction("Dashboard", user.RoleId == "1" ? "Admin" : "User");
			}
			ViewBag.Message = "Invalid email or password.";
			return View();
		}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
