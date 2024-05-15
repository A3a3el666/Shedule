using Microsoft.AspNetCore.Mvc;
using Schedule.Models;

namespace Schedule.Controllers
{
    public class BaseController : Controller
    {
        private readonly IClassRepository _classRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ITeacherSubjectRepository _teacherSubjectRepository;

        public BaseController()
        {
        }

        public BaseController(
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
    }
}
