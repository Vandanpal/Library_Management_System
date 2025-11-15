using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender {  get; set; }
        public int Country_Id { get; set; }
        public int State_Id { get; set; }
        public int City_Id { get; set; }
        public int RoleId { get; set; }
        public string ContactNo {  get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int IsDeleted { get; set; } = 0;
        public Nullable<bool> IsActive { get; set; }
        //public Country country {  get; set; }
        //public State state { get; set; }
        //public City city { get; set; }
        //public Role role { get; set; }
    }
}
