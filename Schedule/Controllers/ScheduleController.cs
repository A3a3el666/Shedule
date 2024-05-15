using Microsoft.AspNetCore.Mvc;
using Schedule.Models;

public class ScheduleController : Controller
{
    private readonly IClassRepository _classRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ITeacherSubjectRepository _teacherSubjectRepository;

    public ScheduleController(
        IClassRepository classRepository,
        ISubjectRepository subjectRepository,
        IScheduleRepository scheduleRepository,
        ITeacherRepository teacherRepository,
        ITeacherSubjectRepository teacherSubjectRepository)
    {
        _classRepository = classRepository;
        _subjectRepository = subjectRepository;
        _scheduleRepository = scheduleRepository;
        _teacherRepository = teacherRepository;
        _teacherSubjectRepository = teacherSubjectRepository;
    }
    [HttpGet]
    public IActionResult ViewScheduleForm()
    {
        IEnumerable<Schedule.Models.Schedule> schedules = _scheduleRepository.GetSchedules();
        return View(schedules);
    }

    [HttpGet]
    public IActionResult CreateScheduleForm()
    {
        var classes = _classRepository.GetAllClasses();
        var subjects = _subjectRepository.GetAllSubjects();
        var teachers = _teacherRepository.GetAllTeachers();
        var teacherSubjects = _teacherSubjectRepository.GetAllTeacherSubjects();

        var viewModel = new List<ScheduleViewModel>();

        foreach (var classObj in classes)
        {
            var scheduleViewModel = new ScheduleViewModel
            {
                ClassId = classObj.ClassId,
                ClassName = classObj.ClassNumber,
                SubjectTeachers = classObj.Subjects
                    .SelectMany(s => teacherSubjects
                        .Where(ts => ts.SubjectId == s.SubjectId)
                        .Select(ts => new SubjectTeacherEntry
                        {
                            SubjectId = s.SubjectId,
                            SubjectName = s.SubjectName,
                            TeacherId = ts.TeacherId,
                            TeacherName = teachers.FirstOrDefault(t => t.TeacherId == ts.TeacherId)?.FullName
                        }))
                    .ToList()
            };

            for (int i = 0; i < 5; i++) // 5 дней недели
            {
                string day = ((DayOfWeek)i).ToString();
                scheduleViewModel.Schedule[day] = new List<ScheduleEntry>();

                for (int j = 0; j < 7; j++) // 7 временных интервалов
                {
                    scheduleViewModel.Schedule[day].Add(new ScheduleEntry
                    {
                        StartTime = new TimeSpan(8 + j, 0, 0),
                        EndTime = new TimeSpan(9 + j, 0, 0)
                    });
                }
            }

            viewModel.Add(scheduleViewModel);
        }

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult SaveSchedule(List<ScheduleViewModel> viewModel)
    {
        foreach (var classSchedule in viewModel)
        {
            var schedule = new Dictionary<string, List<ScheduleEntry>>();

            foreach (var day in classSchedule.Schedule.Keys)
            {
                schedule[day] = classSchedule.Schedule[day].Select(entry => new ScheduleEntry
                {
                    SubjectId = entry.SubjectId,
                    TeacherId = entry.TeacherId,
                    StartTime = entry.StartTime,
                    EndTime = entry.EndTime
                }).ToList();
            }

            _scheduleRepository.SaveSchedule(classSchedule.ClassId, schedule);
        }

        return RedirectToAction("AllClasses", "Class");
    }
}

