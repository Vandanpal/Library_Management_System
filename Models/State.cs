using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class State
    {
        [Key]
        public int State_Id { get; set; }
        public string StateName { get; set; }
        public int c_Id { get; set; }
    }
}
