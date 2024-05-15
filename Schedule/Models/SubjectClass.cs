using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Schedule.Models
{
    [Table("class_subject")]
    public class SubjectClass
    {
        [Key]
        [Column("subject_id", Order = 1)]
        public int SubjectId { get; set; }

        [Key]
        [Column("class_id", Order = 2)]
        public int ClassId { get; set; }

        [Column("hours_per_week")] 
        public int HoursPerWeek { get; set; }
    }
}
