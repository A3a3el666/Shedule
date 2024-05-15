using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Schedule.Models
{
    public class ScheduleViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public List<SubjectTeacherEntry> SubjectTeachers { get; set; }
        public Dictionary<string, List<ScheduleEntry>> Schedule { get; set; }

        public ScheduleViewModel()
        {
            Schedule = new Dictionary<string, List<ScheduleEntry>>();
        }
    }

    public class SubjectTeacherEntry
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
    }

    public class ScheduleEntry
    {
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string SubjectName { get; set; }
    }
}
