namespace Library_Management_System.Models
{
	public class ViewModel
	{
		public int Id { get; set; }
		public string ISBN { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Category { get; set; }
		public bool isAvailable { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public String CreatedBy { get; set; }
	}
}
