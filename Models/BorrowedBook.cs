using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
	public class BorrowedBook
	{
		[Key]
		public int BorrowId { get; set; }
		public int BookId { get; set; }
		public int UserId { get; set; }
		public bool IsBorrowed { get; set; }
		public bool IsActive { get; set; }
		public DateTime BorrowedDate { get; set; }
		public DateTime ReturnDate { get; set; }
		public bool ReminderShown { get; set; } = false;
		public bool IsOverdueNotified { get; set; } = false;
	}
}
