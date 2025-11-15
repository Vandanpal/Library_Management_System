namespace Library_Management_System.Models
{
	public class ReturnViewModel
	{
		public int ReturnId { get; set; }
		public int BookId { get; set; }

		public string UserId { get; set; }
		public bool IsReturned { get; set; }
		public DateTime ReturnedDate { get; set; }
		public bool IsActive { get; set; }
	}
}
