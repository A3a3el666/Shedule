using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models
{
    [Table("schedules")]
    public class Schedule
    {
        [Column("schedule_id")]
        public int ScheduleId { get; set; }

        [Column("class_id")]
        public int ClassId { get; set; }

        [Column("day_of_week")]
        public string DayOfWeek { get; set; }

        [Column("subject_id")]
        public int SubjectId { get; set; }

        [Column("teacher_id")]
        public int TeacherId { get; set; }

        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
    }

}
