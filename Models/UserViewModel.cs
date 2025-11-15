using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library_Management_System.Models
{
    public class UserViewModel
    {
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> CityList { get; set; }
        public List<SelectListItem> RoleList { get; set; }
        public User Users { get; set; }
        public List<User> UserList { get; set; }
        public Login Login { get; set; }
    }
}
