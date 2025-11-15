using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
	public class Returned
	{
		[Key]
		public int ReturnId {  get; set; }
		public int BookId { get; set; }

		public int UserId { get; set; }
		public bool IsReturned { get; set; }
		public DateTime ReturnedDate { get; set; }
		public bool IsActive { get; set; }
	}
}
