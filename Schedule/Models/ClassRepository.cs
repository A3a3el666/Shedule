using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Schedule.Models;

namespace Schedule.Models
{
    public interface IClassRepository
    {
        IEnumerable<Class> GetAllClasses();
    }
    public class ClassRepository : IClassRepository
    {

        private readonly string _connectionString;

        public ClassRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Class> GetAllClasses()
        {
            var classes = new List<Class>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"SELECT c.class_id, c.class_number, s.subject_id, s.subject_name, cs.hour_per_week
                                                 FROM class c
                                                 LEFT JOIN class_subject cs ON c.class_id = cs.class_id
                                                 LEFT JOIN subject s ON cs.subject_id = s.subject_id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var classId = reader.GetInt32(reader.GetOrdinal("class_id"));
                        var @class = classes.FirstOrDefault(c => c.ClassId == classId);
                        if (@class == null)
                        {
                            @class = new Class
                            {
                                ClassId = classId,
                                ClassNumber = reader.GetString(reader.GetOrdinal("class_number")),
                                Subjects = new List<Subject>()
                            };
                            classes.Add(@class);
                        }

                        var subjectId = reader.IsDBNull(reader.GetOrdinal("subject_id")) ? -1 : reader.GetInt32(reader.GetOrdinal("subject_id"));
                        if (subjectId != -1)
                        {
                            var subject = new Subject
                            {
                                SubjectId = subjectId,
                                SubjectName = reader.GetString(reader.GetOrdinal("subject_name")),
                                HoursPerWeek = reader.IsDBNull(reader.GetOrdinal("hour_per_week")) ? 0 : reader.GetInt32(reader.GetOrdinal("hour_per_week"))
                            };
                            @class.Subjects.Add(subject);
                        }
                    }
                }
            }

            return classes;
        }



        //-------------------------------------------------------
        public void AddClass(string classNumber)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO class (class_number) VALUES (@ClassNumber)", connection))
                {
                    command.Parameters.AddWithValue("@ClassNumber", classNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        //------------------------------------------
        public void DeleteClass(int classId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Удаление связей класса с учителями
                using (var deleteTeacherClassCommand = new NpgsqlCommand("DELETE FROM teacher_class WHERE class_id = @ClassId", connection))
                {
                    deleteTeacherClassCommand.Parameters.AddWithValue("@ClassId", classId);
                    deleteTeacherClassCommand.ExecuteNonQuery();
                }

                // Удаление связей класса с предметами
                using (var deleteClassSubjectCommand = new NpgsqlCommand("DELETE FROM class_subject WHERE class_id = @ClassId", connection))
                {
                    deleteClassSubjectCommand.Parameters.AddWithValue("@ClassId", classId);
                    deleteClassSubjectCommand.ExecuteNonQuery();
                }

                // Удаление класса
                using (var deleteClassCommand = new NpgsqlCommand("DELETE FROM class WHERE class_id = @ClassId", connection))
                {
                    deleteClassCommand.Parameters.AddWithValue("@ClassId", classId);
                    deleteClassCommand.ExecuteNonQuery();
                }
            }
        }

        //------------------------------------------
        public void AddSubjectToClass(int classId, int[] subjectIds, int[] hoursPerWeek)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var deleteCommand = new NpgsqlCommand("DELETE FROM class_subject WHERE class_id = @ClassId", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@ClassId", classId);
                    deleteCommand.ExecuteNonQuery();
                }

                for (int i = 0; i < subjectIds.Length; i++)
                {
                    int subjectId = subjectIds[i];
                    int hourPerWeek = hoursPerWeek[i];

                    if (hourPerWeek > 0)
                    {
                        using (var insertCommand = new NpgsqlCommand("INSERT INTO class_subject (class_id, subject_id, hour_per_week) VALUES (@ClassId, @SubjectId, @HourPerWeek)", connection))
                        {
                            insertCommand.Parameters.AddWithValue("@ClassId", classId);
                            insertCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                            insertCommand.Parameters.AddWithValue("@HourPerWeek", hourPerWeek);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }





        //------------------------------------------

        public Class GetClassById(int classId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM class WHERE class_id = @ClassId", connection))
                {
                    command.Parameters.AddWithValue("@ClassId", classId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Class
                            {
                                ClassId = reader.GetInt32(reader.GetOrdinal("class_id")),
                                ClassNumber = reader.GetString(reader.GetOrdinal("class_number")),
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }


        //------------------------------------------
        private string GetTeacherNameById(int teacherId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT full_name FROM teacher WHERE teacher_id = @TeacherId", connection))
                {
                    command.Parameters.AddWithValue("@TeacherId", teacherId);
                    var teacherName = (string)command.ExecuteScalar();
                    return teacherName;
                }
            }
        }
    }
}
