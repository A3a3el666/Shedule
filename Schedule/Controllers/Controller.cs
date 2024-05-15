using Microsoft.AspNetCore.Mvc;
using Schedule.Models;

namespace Schedule.Controllers
{
    public class BaseController : Controller
    {
        private readonly ClassRepository _classRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly TeacherRepository _teacherRepository;
        private readonly TeacherSubjectRepository _teacherSubjectRepository;

        public BaseController()
        {
        }

        public BaseController(
            ClassRepository classRepository,
            SubjectRepository subjectRepository,
            TeacherRepository teacherRepository,
            TeacherSubjectRepository teacherSubjectRepository)
        {
            _classRepository = classRepository;
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
        }
    }
}
