using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class City
    {
        [Key]
        public int City_Id { get; set; }
        public string CityName { get; set; }
        public int s_Id { get; set; }
    }
}
