namespace Library_Management_System.Models
{
	public class RenewViewModel
	{
		public int RenewId { get; set; }
		public int BookId { get; set; }
		public string UserId { get; set; }

		public bool IsRenew { get; set; }
		public DateTime RenewDate { get; set; }
		public DateTime ReturnDate { get; set; }
		public bool IsActive { get; set; }
	}
}
