using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Schedule.Models
{
    [Table("teachers")]
    public class Teacher
    {
        [Key]
        [Column("teacher_id")]
        public int TeacherId { get; set; }

        [Required]
        [Column("full_name")]
        public string FullName { get; set; }

        [Column("room_number")]
        public string RoomNumber { get; set; }
        //-----------------------------------------------
        public List<Class> Classes { get; set; } 
        public List<Subject> Subjects { get; set; }

        public Teacher()
        {
            Classes = new List<Class>(); 
            Subjects = new List<Subject>();
        }
    }
}
