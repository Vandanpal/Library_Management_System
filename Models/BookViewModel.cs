using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library_Management_System.Models
{
    public class BookViewModel
    {
        public Book Book {  get; set; }
        public List<Book> BookList { get; set; }
        public Login tblLogin { get; set; }
    }
}
