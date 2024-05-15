using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Schedule.Models
{
    [Table("teacher_subject")]
    public class TeacherSubject
    {
        [Key]
        [Column("teacher_id", Order = 1)] 
        public int TeacherId { get; set; }

        [Key]
        [Column("subject_id", Order = 2)] 
        public int SubjectId { get; set; }
    }
}
