using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Schedule.Models
{
    [Table("subjects")]
    public class Subject
    {
        [Key]
        [Column("subject_id")]
        public int SubjectId { get; set; }

        [Required]
        [Column("subject_name")]
        public string SubjectName { get; set; }

        public int HoursPerWeek { get; set; }
    }
}
