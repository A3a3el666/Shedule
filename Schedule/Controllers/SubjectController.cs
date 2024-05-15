using Microsoft.AspNetCore.Mvc;
using Schedule.Models;

namespace Schedule.Controllers
{
    public class SubjectController : Controller
    {
        private readonly SubjectRepository _subjectRepository;

        public SubjectController(SubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }
        [HttpGet]
        public IActionResult AllSubjectsForm()
        {
            var subjects = _subjectRepository.GetAllSubjects();
            return View(subjects);
        }
        [HttpGet]
        public IActionResult AddSubjectForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _subjectRepository.AddSubject(subject);
                return RedirectToAction("AllSubjectsForm");
            }
            return View(subject);
        }

        [HttpGet]
        public IActionResult DeleteSubjectForm()
        {
            var subjects = _subjectRepository.GetAllSubjects();
            return View("DeleteSubjectForm", subjects);
        }

        [HttpPost]
        public IActionResult DeleteSubject(int subjectId)
        {
            _subjectRepository.DeleteSubject(subjectId);
            return RedirectToAction("AllSubjectsForm");
        }


    }
}

