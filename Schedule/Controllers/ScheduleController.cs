using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Schedule.Data;
using Schedule.Models;
using Schedule.Repositories;

namespace Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ScheduleRepository _scheduleRepository;
        private readonly ApplicationDbContext _context;

        public ScheduleController(ScheduleRepository scheduleRepository, ApplicationDbContext context)
        {
            _scheduleRepository = scheduleRepository;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateScheduleForm()
        {
            ViewBag.ClassList = new SelectList(_context.Classes, "ClassId", "ClassName");
            return View();
        }

        [HttpPost]
        public IActionResult GenerateSchedule(int classId)
        {
            var schedule = _scheduleRepository.GenerateSchedule(classId);
            return View("Index", schedule);
        }
    }
}
