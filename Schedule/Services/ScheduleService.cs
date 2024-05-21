using Schedule.Data;

namespace Schedule.Services {
public class ScheduleService
{
    private readonly SchoolContext _context;

    public ScheduleService(SchoolContext context)
    {
        _context = context;
    }

    public List<SchoolSchedule> GenerateSchedule()
    {
        var schedule = new List<SchoolSchedule>();

        // Получаем все необходимые данные из базы
        var classes = _context.Class.ToList();
        var subjects = _context.Subject.ToList();
        var classSubjects = _context.ClassSubject.ToList();
        var teacherClasses = _context.TeacherClass.ToList();
        var teacherSubjects = _context.TeacherSubject.ToList();
        var teachers = _context.Teacher.ToList();

        // Пример простого алгоритма для распределения уроков
        foreach (var classSubject in classSubjects)
        {
            var classId = classSubject.ClassId;
            var subjectId = classSubject.SubjectId;
            var hoursPerWeek = classSubject.HoursPerWeek;

            var teacherId = teacherSubjects
                .FirstOrDefault(ts => ts.SubjectId == subjectId)?.TeacherId;

            if (teacherId != null)
            {
                for (int i = 0; i < hoursPerWeek; i++)
                {
                    schedule.Add(new SchoolSchedule
                    {
                        ClassId = classId,
                        SubjectId = subjectId,
                        TeacherId = teacherId.Value,
                        DayOfWeek = (DayOfWeek)(i % 5), // Примерное распределение по дням недели
                        Period = i // Примерное распределение по урокам в день
                    });
                }
            }
        }

        return schedule;
    }
}

public class SchoolSchedule
{
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int Period { get; set; }
}
}
