using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Models;

namespace Schedule.Repositories
{
    public class ScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ScheduleEntry> GenerateSchedule(int classId)
        {
            var classObj = _context.Classes
                .Include(c => c.Subjects)
                .FirstOrDefault(c => c.ClassId == classId);

            if (classObj == null)
                throw new InvalidOperationException("Class not found");

            var schedule = new List<ScheduleEntry>();

            foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>())
            {
                foreach (var subject in classObj.Subjects)
                {
                    var randomTeacher = _context.Teachers.OrderBy(t => Guid.NewGuid()).FirstOrDefault();
                    if (randomTeacher == null)
                        throw new InvalidOperationException("No teachers found");

                    var startTime = new TimeSpan(8, 0, 0); // Start at 8:00 AM
                    var endTime = new TimeSpan(9, 0, 0);   // End at 9:00 AM

                    var entry = new ScheduleEntry
                    {
                        ClassId = classObj.ClassId,
                        SubjectId = subject.SubjectId,
                        TeacherId = randomTeacher.TeacherId,
                        DayOfWeek = dayOfWeek,
                        StartTime = startTime,
                        EndTime = endTime
                    };

                    schedule.Add(entry);
                }
            }

            return schedule;
        }
    }
}
