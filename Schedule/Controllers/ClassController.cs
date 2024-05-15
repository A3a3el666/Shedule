using Microsoft.AspNetCore.Mvc;
using Schedule.Models;

namespace Schedule.Controllers
{
    public class ClassController : Controller
    {
        private readonly ClassRepository _classRepository;
        private readonly SubjectRepository _subjectRepository;

        public ClassController(ClassRepository classRepository, SubjectRepository subjectRepository)
        {
            _classRepository = classRepository;
            _subjectRepository = subjectRepository;
        }

        [HttpGet]
        public IActionResult AllClassesForm()
        {
            var classes = _classRepository.GetAllClasses();
            return View(classes);
        }

        [HttpGet]
        public IActionResult AddClassForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddClass(string classNumber)
        {
            _classRepository.AddClass(classNumber);
            return RedirectToAction("AllClassesForm");
        }

        [HttpGet]
        public IActionResult DeleteClassForm()
        {
            var classes = _classRepository.GetAllClasses();
            return View("DeleteClassForm", classes);
        }

        [HttpPost]
        public IActionResult DeleteClass(int classId)
        {
            _classRepository.DeleteClass(classId);
            return RedirectToAction("AllClassesForm");
        }
        [HttpGet]
        public IActionResult AddSubjectToClassForm()
        {
            var classes = _classRepository.GetAllClasses();
            var subjects = _subjectRepository.GetAllSubjects();

            ViewBag.Classes = classes;
            ViewBag.Subjects = subjects;

            return View();
        }

        [HttpPost]
        public IActionResult AddSubjectToClass(int classId, int[] subjectIds, Dictionary<int, int> subjectHours)
        {
            if (subjectIds == null || subjectIds.Length == 0)
            {
                ModelState.AddModelError("", "Выберите хотя бы один предмет");
                return RedirectToAction("AddSubjectToClassForm");
            }

            var classObj = _classRepository.GetClassById(classId);
            if (classObj == null)
            {
                return NotFound();
            }

            // Фильтрация предметов с нулевыми часами
            var filteredSubjectIds = subjectIds.Where(id => subjectHours.ContainsKey(id) && subjectHours[id] > 0).ToArray();
            var filteredHoursPerWeek = filteredSubjectIds.Select(id => subjectHours[id]).ToArray();

            if (filteredSubjectIds.Length == 0)
            {
                ModelState.AddModelError("", "Выберите хотя бы один предмет с ненулевым количеством часов");
                return RedirectToAction("AddSubjectToClassForm");
            }

            _classRepository.AddSubjectToClass(classId, filteredSubjectIds, filteredHoursPerWeek);

            return RedirectToAction("AllClassesForm", "Class");
        }






    }
}

