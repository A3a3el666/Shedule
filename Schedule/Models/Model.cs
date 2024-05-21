using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models
{
    [Table("class")]
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        public string ClassNumber { get; set; }

        public ICollection<ClassSubject> ClassSubjects { get; set; }

        public Class()
        {
            ClassSubjects = new List<ClassSubject>();
        }
    }

    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public ICollection<ClassSubject> ClassSubjects { get; set; }

        public ICollection<TeacherSubject> TeacherSubjects { get; set; }

        public Subject()
        {
            ClassSubjects = new List<ClassSubject>();
            TeacherSubjects = new List<TeacherSubject>();
        }
    }

    public class ClassSubject
    {
        [Key, Column(Order = 0)]
        public int ClassId { get; set; }

        [Key, Column(Order = 1)]
        public int SubjectId { get; set; }

        public int HoursPerWeek { get; set; }

        [ForeignKey("ClassId")]
        public Class Class { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }

        //----------------------
        [NotMapped]
        public string SubjectName { get; set; }
    }

    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        public string FullName { get; set; }

        public string RoomNumber { get; set; }

        public ICollection<Class> TeacherClasses { get; set; }

        public ICollection<Subject> TeacherSubjects { get; set; }

        public Teacher()
        {
            TeacherClasses = new List<Class>();
            TeacherSubjects = new List<Subject>();
        }
    }

    public class TeacherClass
    {
        [Key, Column(Order = 0)]
        public int TeacherId { get; set; }

        [Key, Column(Order = 1)]
        public int ClassId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }

        [ForeignKey("ClassId")]
        public Class Class { get; set; }

    }

    public class TeacherSubject
    {
        [Key, Column(Order = 0)]
        public int TeacherId { get; set; }

        [Key, Column(Order = 1)]
        public int SubjectId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
    }
}
