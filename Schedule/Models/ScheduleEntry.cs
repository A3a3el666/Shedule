using Schedule.Models;

public class ScheduleEntry
{
    public int ScheduleEntryId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public Class Class { get; set; }
    public Subject Subject { get; set; }
    public Teacher Teacher { get; set; }
}