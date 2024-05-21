using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Schedule.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=5432;Database=schedule;User Id=postgres;Password=123;";

        // Метод для отображения страницы с кнопкой
        public IActionResult CreateScheduleForm()
        {
            return View();
        }

        // Метод для генерации и отображения расписания
        [HttpPost]
        public IActionResult GenerateSchedule()
        {
            var scheduleViewModel = GenerateWeeklySchedule();
            return View("ViewScheduleForm", scheduleViewModel);
        }

        private ScheduleViewModel GenerateWeeklySchedule()
        {
            var classSubjects = GetClassSubjects();
            var teacherSubjects = GetTeacherSubjects();
            var teacherClasses = GetTeacherClasses();
            var classes = GetClasses();

            var scheduleEntries = new List<ScheduleEntry>();

            // Слоты расписания
            var timeIntervals = new List<TimeInterval>
            {
                new TimeInterval { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0) },
                new TimeInterval { StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0) },
                new TimeInterval { StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 0, 0) },
                new TimeInterval { StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(13, 0, 0) },
                new TimeInterval { StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(14, 0, 0) },
                new TimeInterval { StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 0, 0) },
                new TimeInterval { StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(16, 0, 0) },
                // Добавьте остальные временные интервалы
            };

            var daysOfWeek = new[]
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday
            };

            var classSubjectHours = classSubjects.ToDictionary(cs => new { cs.ClassId, cs.SubjectId }, cs => cs.HoursPerWeek);

            foreach (var classSubject in classSubjects)
            {
                var teacher = teacherSubjects.FirstOrDefault(ts => ts.SubjectId == classSubject.SubjectId)?.TeacherId;

                if (teacher != null)
                {
                    var hoursScheduled = 0;
                    foreach (var day in daysOfWeek)
                    {
                        foreach (var interval in timeIntervals)
                        {
                            if (hoursScheduled >= classSubject.HoursPerWeek)
                                break;

                            if (!IsSlotOccupied(scheduleEntries, classSubject.ClassId, teacher.Value, day, interval))
                            {
                                var subjectName = GetSubjectName(classSubject.SubjectId);
                                var teacherName = GetTeacherName(teacher.Value);
                                var classNumber = classes.FirstOrDefault(c => c.ClassId == classSubject.ClassId)?.ClassNumber;

                                scheduleEntries.Add(new ScheduleEntry
                                {
                                    ClassNumber = classNumber,
                                    SubjectName = subjectName,
                                    TeacherName = teacherName,
                                    DayOfWeek = day,
                                    StartTime = interval.StartTime,
                                    EndTime = interval.EndTime
                                });

                                hoursScheduled++;
                            }
                        }

                        if (hoursScheduled >= classSubject.HoursPerWeek)
                            break;
                    }
                }
            }

            return new ScheduleViewModel
            {
                Entries = scheduleEntries,
                TimeIntervals = timeIntervals
            };
        }

        private bool IsSlotOccupied(List<ScheduleEntry> scheduleEntries, int classId, int teacherId, DayOfWeek day, TimeInterval interval)
        {
            return scheduleEntries.Any(se =>
                (se.ClassNumber == GetClassNumber(classId) || se.TeacherName == GetTeacherName(teacherId)) &&
                se.DayOfWeek == day &&
                se.StartTime == interval.StartTime &&
                se.EndTime == interval.EndTime);
        }

        private List<ClassSubject> GetClassSubjects()
        {
            var classSubjects = new List<ClassSubject>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT class_id, subject_id, hour_per_week FROM class_subject", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            classSubjects.Add(new ClassSubject
                            {
                                ClassId = reader.GetInt32(0),
                                SubjectId = reader.GetInt32(1),
                                HoursPerWeek = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return classSubjects;
        }

        private List<TeacherSubject> GetTeacherSubjects()
        {
            var teacherSubjects = new List<TeacherSubject>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT teacher_id, subject_id FROM teacher_subject", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            teacherSubjects.Add(new TeacherSubject
                            {
                                TeacherId = reader.GetInt32(0),
                                SubjectId = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }
            return teacherSubjects;
        }

        private List<TeacherClass> GetTeacherClasses()
        {
            var teacherClasses = new List<TeacherClass>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT teacher_id, class_id FROM teacher_class", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            teacherClasses.Add(new TeacherClass
                            {
                                TeacherId = reader.GetInt32(0),
                                ClassId = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }
            return teacherClasses;
        }

        private List<Class> GetClasses()
        {
            var classes = new List<Class>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT class_id, class_number FROM class", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            classes.Add(new Class
                            {
                                ClassId = reader.GetInt32(0),
                                ClassNumber = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return classes;
        }

        private string GetSubjectName(int subjectId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT subject_name FROM subject WHERE subject_id = @subjectId", connection))
                {
                    command.Parameters.AddWithValue("@subjectId", subjectId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
        }

        private string GetTeacherName(int teacherId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT full_name FROM teacher WHERE teacher_id = @teacherId", connection))
                {
                    command.Parameters.AddWithValue("@teacherId", teacherId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
        }

        private string GetClassNumber(int classId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT class_number FROM class WHERE class_id = @classId", connection))
                {
                    command.Parameters.AddWithValue("@classId", classId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
        }
    }
}
