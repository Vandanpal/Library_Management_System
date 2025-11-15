using Library_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.DataConnection
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        }
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> TblUser { get; set; }
        public DbSet<Login> TblLogin { get; set; }
        public DbSet<Book> TblBooks { get; set; }
		public DbSet<BorrowedBook> Transaction_Log { get; set; }
        public DbSet<Returned> ReturnBookLog { get; set; }
		public DbSet<RenewBook> RenewBookLog { get; set; }
	}
}
