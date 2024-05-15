using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Schedule.Models
{
    [Table("classes")]
    public class Class
    {
        [Key]
        [Column("class_id")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите номер класса")]
        [Column("class_number")]
        public string ClassNumber { get; set; }

        public List<Subject> Subjects { get; set; }

        public Class()
        {
            Subjects = new List<Subject>();
        }
    }
}
