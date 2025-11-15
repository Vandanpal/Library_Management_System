using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library_Management_System.Models
{
	public class RenewBook
	{
		[Key]
		public int RenewId { get; set; }
		public int BookId { get; set; }
		public int UserId { get; set; }

		public bool IsRenew { get; set; }
		public DateTime RenewDate { get; set; }
		public DateTime ReturnDate { get; set; }
		public bool IsActive { get; set; }
	}
}
