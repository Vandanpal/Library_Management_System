using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Password {  get; set; }
        public int LoginId {  get; set; }
        public string RoleId { get; set; }
        public Nullable<bool> IsActive { get; set; }

    }
}
