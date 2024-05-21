using System;
using System.Collections.Generic;

namespace Schedule.Models
{
    public class ScheduleViewModel
    {
        public List<ScheduleEntry> Entries { get; set; }
        public List<TimeInterval> TimeIntervals { get; set; }
    }

    public class ScheduleEntry
    {
        public string ClassNumber { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class TimeInterval
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
