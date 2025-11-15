using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class Country
    {
        [Key]
        public int Country_Id { get; set; }
        public string CountryName { get; set; }
    }
}
