using Library_Management_System.DataConnection;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;

namespace Library_Management_System.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            string userId = HttpContext.Session.GetString("LoginId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }
            int userid = Convert.ToInt32(userId);
            var totalbookcount = _context.TblBooks.Where(b => b.isAvailable).Count();
            ViewBag.totalbookcount = totalbookcount;
            var borrowbookcount = _context.Transaction_Log.Where(b => b.UserId == userid && b.IsActive).Count();
            ViewBag.borrowbookcount = borrowbookcount;

            var returnbookcount = _context.ReturnBookLog.Where(r => r.UserId == userid && r.IsActive).Count();
            ViewBag.returnbookcount = returnbookcount;
            return View();
        }
        public IActionResult BookList()
        {
			var books = _context.TblBooks.Where(b=>b.isAvailable).ToList();
			return View(books);
        }
        [HttpGet]
		public IActionResult BorrowedList()
		{
			string userId = HttpContext.Session.GetString("LoginId");
			if (string.IsNullOrEmpty(userId))
			{
				return RedirectToAction("Login");
			}
			int userid = Convert.ToInt32(userId);
			DateTime today = DateTime.Today;
			DateTime maxDueDate = today.AddDays(3);
			var borrowbooks = _context.Transaction_Log.Where(b =>b.UserId==userid && b.IsActive).ToList();

			var reminders = borrowbooks.Where(t => !t.ReminderShown && t.ReturnDate.Date <= maxDueDate && t.ReturnDate.Date >= today).ToList();
			foreach (var item in reminders)
			{
				item.ReminderShown = true;
			}

			var overdueBooks = borrowbooks.Where(t => t.ReturnDate.Date < today && !t.IsOverdueNotified).ToList();
			foreach (var book in overdueBooks)
			{
				book.IsOverdueNotified = true;
			}
			_context.SaveChanges();

			ViewBag.Reminders = reminders;
			ViewBag.OverdueBooks = overdueBooks;
			return View(borrowbooks);
		}

		[HttpPost]
		public IActionResult BorrowedList(int Id,DateTime ReturnDate)
        {
            var book=_context.TblBooks.FirstOrDefault(b=>b.Id==Id);
			if (book == null)
			{
				return NotFound("Book not found.");
			}
			string userIdStr = HttpContext.Session.GetString("LoginId");
			if (string.IsNullOrEmpty(userIdStr))
			{
				return Content("User session expired. Please login again.");
			}

			int userId = Convert.ToInt32(userIdStr);

            var model = new BorrowedBook
            {
				UserId = userId,
			    BookId = book.Id,
			    IsBorrowed = true,
			    IsActive = true,
			    BorrowedDate = DateTime.Now,
				ReturnDate= ReturnDate
			};
            _context.Transaction_Log.Add(model);
			book.isAvailable = false;
			_context.TblBooks.Update(book);
			_context.SaveChanges();
			return RedirectToAction("BorrowedList");
		}
        [HttpGet]
        public IActionResult ReturnedList()
        {
            string userId = HttpContext.Session.GetString("LoginId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }
            int userid = Convert.ToInt32(userId);
			var ReturnedList = _context.ReturnBookLog.Where(r=>r.UserId==userid && r.IsActive).ToList();
			return View(ReturnedList);
        }
		[HttpPost]
		public IActionResult ReturnedList(int BorrowId)
		{
            var borrowedbooks= _context.Transaction_Log.FirstOrDefault(b=>b.BorrowId==BorrowId);
			if (borrowedbooks == null)
			{
				return NotFound("Borrowed record not found.");
			}
			string userIdStr = HttpContext.Session.GetString("LoginId");
			if (string.IsNullOrEmpty(userIdStr))
			{
				return Content("User session expired. Please login again.");
			}

			int userId = Convert.ToInt32(userIdStr);

			var model = new Returned
			{
				UserId = userId,
				BookId = borrowedbooks.BookId,
				IsReturned = true,
				IsActive = true,
				ReturnedDate = DateTime.Now
			};
			_context.ReturnBookLog.Add(model);

			var book = _context.TblBooks.FirstOrDefault(b => b.Id == borrowedbooks.BookId);
			if (book != null)
			{
				book.isAvailable = true;
				_context.TblBooks.Update(book);
			}

			borrowedbooks.IsActive = false;
			_context.Transaction_Log.Update(borrowedbooks);


			var renewBook=_context.RenewBookLog.FirstOrDefault(b => b.RenewId==borrowedbooks.BookId);
            if (renewBook != null)
            {
                renewBook.IsActive = false;
                _context.RenewBookLog.Update(renewBook);
            }
   //         renewBook.IsActive = false;
			//_context.RenewBookLog.Update(renewBook);
			_context.SaveChanges();
			return RedirectToAction("ReturnedList");
		}
		[HttpGet]
		public IActionResult RenewBorrowedBook()
		{
			string userId = HttpContext.Session.GetString("LoginId");
			if (string.IsNullOrEmpty(userId))
			{
				return RedirectToAction("Login");
			}
			int userid = Convert.ToInt32(userId);
			var renewbooks = _context.RenewBookLog.Where(r => r.UserId == userid && r.IsActive).ToList();
			return View(renewbooks);
		}
		[HttpPost]
		public IActionResult RenewBorrowedBook(int BorrowId ,DateTime ReturnDate)
        {
			var borrowedbooks = _context.Transaction_Log.FirstOrDefault(b => b.BorrowId == BorrowId);
			if (borrowedbooks == null)
			{
				return NotFound("Borrowed record not found.");
			}
			string userIdStr = HttpContext.Session.GetString("LoginId");
			if (string.IsNullOrEmpty(userIdStr))
			{
				return Content("User session expired. Please login again.");
			}

			int userId = Convert.ToInt32(userIdStr);

			var model = new RenewBook
			{
				UserId = userId,
				BookId = borrowedbooks.BookId,
				IsRenew = true,
				IsActive = true,
				RenewDate = DateTime.Now,
				ReturnDate= ReturnDate
			};
			_context.RenewBookLog.Add(model);

			
			borrowedbooks.ReturnDate = ReturnDate;
			_context.Transaction_Log.Update(borrowedbooks);

			_context.SaveChanges();
			return RedirectToAction("BorrowedList");
		}
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
