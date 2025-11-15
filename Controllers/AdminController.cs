using Library_Management_System.DataConnection;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library_Management_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
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
        public IActionResult Dashboard()
        {

            var registerCount = _context.TblUser.Count();
            ViewBag.RegisterCount = registerCount;
            var totalbookcount = _context.TblBooks.Where(b => b.isAvailable).Count();
            ViewBag.totalbookcount = totalbookcount;
            var borrowbookcount = _context.Transaction_Log.Where(b => b.IsActive).Count();
            ViewBag.borrowbookcount = borrowbookcount;

            var returnbookcount = _context.ReturnBookLog.Where(r => r.IsActive).Count();
            ViewBag.returnbookcount = returnbookcount;
            return View();
        }
        [HttpGet]
        public IActionResult AddBook()
        {
            var books = _context.TblBooks.Where(b => b.isAvailable).ToList();
			var model = new BookViewModel
            {
                Book=new Book(),
                BookList=books
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult AddBook(BookViewModel bvm)
        {
			string userIdStr = HttpContext.Session.GetString("LoginId");

			if (string.IsNullOrEmpty(userIdStr))
			{
				return Content("User session not found. Please login again.");
			}

			int userId = Convert.ToInt32(userIdStr);

            var user = _context.TblUser.FirstOrDefault(u => u.Id == userId);
			if (user == null)
			{
				return Content("User not found in database. Please check.");
			}

			if (bvm.Book.Id==0)
            {
                bvm.Book.CreatedBy = userId;
				bvm.Book.CreatedOn = DateTime.Now;
                bvm.Book.isAvailable = true;
                _context.TblBooks.Add(bvm.Book);
            }
            else
            {
                _context.TblBooks.Update(bvm.Book);
            }
            _context.SaveChanges();
            return RedirectToAction("AddBook");
        }
       
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var model = new BookViewModel();
            var book=_context.TblBooks.FirstOrDefault(b=>b.Id == Id);
            var bookView = new BookViewModel
            {
                Book=book,
                BookList=_context.TblBooks.ToList()
            };
            if (book == null)
            {
                NotFound(); 
            }
            return View("AddBook", bookView);
        }
        public IActionResult Delete(int id)
        {
            var book = _context.TblBooks.FirstOrDefault(b => b.Id == id);
            if (book != null) 
            {
				book.isAvailable = false;
				_context.TblBooks.Update(book);
				_context.SaveChanges();
            }
            return RedirectToAction("AddBook");
        }
        [HttpGet]
        public IActionResult BorrowList()
        {
            var borrowbooks = _context.Transaction_Log.Where(b => b.IsActive).ToList();
            return View(borrowbooks);
        }
        [HttpGet]
        public IActionResult ReturnList()
        {
            var ReturnedList = _context.ReturnBookLog.Where(r => r.IsActive).ToList();
            return View(ReturnedList);
        }
		[HttpGet]
		public IActionResult RenewList()
		{
			var RenewRecords = _context.RenewBookLog.Where(r => r.IsActive).ToList();
			return View(RenewRecords);
		}
		[HttpGet]
        public IActionResult User()
        {
            var model = new UserViewModel();
            model.CountryList = _context.Country.
                Select(c => new SelectListItem
                {
                    Value = c.Country_Id.ToString(),
                    Text = c.CountryName
                }).ToList();

            model.RoleList = _context.Role.
                Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.RoleName
                }).ToList();

            var User = _context.TblUser.Where(u => u.IsDeleted == 0).ToList();
            var viewmodel = new UserViewModel
            {
                UserList=User,
                CountryList = model.CountryList,
                RoleList = model.RoleList
            };

            return View(viewmodel);
        }
        [HttpPost]
        public IActionResult User(UserViewModel uvm)
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
				var user = _context.TblUser.FirstOrDefault(u => u.Id ==uvm.Users.Id);
                //uvm.Users.CreatedDate = user.CreatedDate;
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
            return RedirectToAction("User");
        }
        [HttpGet]
        public IActionResult UserList()
        {
            var users = _context.TblUser.Where(u => u.IsDeleted == 0).ToList();
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }
        [HttpGet]
        public IActionResult EditUser(int Id)
        {
            var user = _context.TblUser.FirstOrDefault(u => u.Id == Id);
            var model = new UserViewModel();
            model.CountryList = _context.Country.
                Select(c => new SelectListItem
                {
                    Value = c.Country_Id.ToString(),
                    Text = c.CountryName
                }).ToList();

            model.StateList = _context.State
                .Where(s => s.c_Id == user.Country_Id)
                .Select(s => new SelectListItem
                {
                    Value = s.State_Id.ToString(),
                    Text = s.StateName
                })
                .ToList();

            model.CityList = _context.City
                .Where(s => s.s_Id == user.State_Id)
                .Select(s => new SelectListItem
                {
                    Value = s.City_Id.ToString(),
                    Text = s.CityName
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
                Users = user,
                UserList = _context.TblUser.ToList(),
                CountryList = model.CountryList,
                RoleList = model.RoleList,
                StateList = model.StateList,
                CityList = model.CityList

            };
            return View("User", viewmodel);
        }
        public IActionResult DeleteUser(int Id)
        {
            var user = _context.TblUser.FirstOrDefault(u => u.Id == Id);
            var u = _context.TblLogin.FirstOrDefault(u => u.LoginId == Id);
            if (user != null)
            {
                u.IsActive = false;
                user.IsDeleted = 1;
                user.IsActive = false;
                _context.SaveChanges();
            }
            return RedirectToAction("User");
        }
        [HttpGet]
        public IActionResult BorrowReport()
        {
            var borrowreport = _context.Transaction_Log.ToList();
            return View(borrowreport);
        }
		[HttpGet]
		public IActionResult ReturnReport()
        {
			var returnreport = _context.ReturnBookLog.ToList();
			return View(returnreport);
        }
		[HttpGet]
		public IActionResult RenewReport()
        {
			var renewreport = _context.RenewBookLog.ToList();
			return View(renewreport);
        }
    }
}
