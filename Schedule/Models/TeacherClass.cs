using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Schedule.Models
{
    [Table("teacher_class")]
    public class TeacherClass
    {
        [Key]
        [Column("teacher_id", Order = 1)]
        public int TeacherId { get; set; }

        [Key]
        [Column("class_id", Order = 2)]
        public int ClassId { get; set; }
    }
}
