using Microsoft.AspNetCore.Mvc;
using Schedule.Models;

namespace Schedule.Controllers
{
    public class TeacherController : Controller
    {
        private readonly TeacherRepository _teacherRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly ClassRepository _classRepository;

        public TeacherController(
            TeacherRepository teacherRepository,
            SubjectRepository subjectRepository,
            ClassRepository classRepository)
        {
            _teacherRepository = teacherRepository;
            _subjectRepository = subjectRepository;
            _classRepository = classRepository;
        }

        public IActionResult AllTeachersForm()
        {
            var teachers = _teacherRepository.GetAllTeachers();
            return View(teachers);
        }

        [HttpGet]
        public IActionResult AddTeacherForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTeacher(string fullName, string roomNumber)
        {
            _teacherRepository.AddTeacher(fullName, roomNumber);
            return RedirectToAction("AllTeachersForm");
        }

        [HttpGet]
        public IActionResult DeleteTeacherForm()
        {
            var teachers = _teacherRepository.GetAllTeachers();
            return View("DeleteTeacherForm", teachers);
        }

        [HttpPost]
        public IActionResult DeleteTeacher(int teacherId)
        {
            _teacherRepository.DeleteTeacher(teacherId);
            return RedirectToAction("AllTeachersForm");
        }

        [HttpGet]
        public IActionResult AddSubjectToTeacherForm()
        {
            var teachers = _teacherRepository.GetAllTeachers();
            var subjects = _subjectRepository.GetAllSubjects();

            ViewBag.Teachers = teachers;
            ViewBag.Subjects = subjects;

            return View();
        }

        [HttpPost]
        public IActionResult AddSubjectToTeacher(int teacherId, int[] selectedSubjects)
        {
            if (selectedSubjects == null || selectedSubjects.Length == 0)
            {
                ModelState.AddModelError("", "Выберите хотя бы один предмет");
                return RedirectToAction("AddSubjectToTeacherForm");
            }

            var teacher = _teacherRepository.GetTeacherById(teacherId);
            if (teacher == null)
            {
                return NotFound();
            }

            _teacherRepository.AddSubjectToTeacher(teacherId, selectedSubjects);

            return RedirectToAction("AllTeachersForm", "Teacher");
        }

        [HttpGet]
        public IActionResult AddClassToTeacherForm()
        {
            var teachers = _teacherRepository.GetAllTeachers();
            var classes = _classRepository.GetAllClasses();

            ViewBag.Teachers = teachers;
            ViewBag.Classes = classes;

            return View();
        }

        [HttpPost]
        public IActionResult AddClassToTeacher(int teacherId, int[] selectedClasses)
        {
            if (selectedClasses == null || selectedClasses.Length == 0)
            {
                ModelState.AddModelError("", "Выберите хотя бы один класс");
                return RedirectToAction("AddClassToTeacherForm");
            }

            var teacher = _teacherRepository.GetTeacherById(teacherId);
            if (teacher == null)
            {
                return NotFound();
            }

            _teacherRepository.AddClassToTeacher(teacherId, selectedClasses);

            return RedirectToAction("AllTeachersForm", "Teacher");
        }
    }
}