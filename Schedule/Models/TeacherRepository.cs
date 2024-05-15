using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Schedule.Models;

namespace Schedule.Models
{
    public interface ITeacherRepository
    {
        IEnumerable<Teacher> GetAllTeachers();
    }
    public class TeacherRepository  : ITeacherRepository
    {
        private readonly string _connectionString;

        public TeacherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        //-------------------------------------------------------------
        public IEnumerable<Teacher> GetAllTeachers()
        {
            var teachersDict = new Dictionary<int, Teacher>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"SELECT t.teacher_id, t.full_name, t.room_number, 
                                            c.class_id, c.class_number, 
                                            s.subject_id, s.subject_name
                                            FROM teacher t
                                            LEFT JOIN teacher_class tc ON t.teacher_id = tc.teacher_id
                                            LEFT JOIN class c ON tc.class_id = c.class_id
                                            LEFT JOIN teacher_subject ts ON t.teacher_id = ts.teacher_id
                                            LEFT JOIN subject s ON ts.subject_id = s.subject_id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var teacherId = reader.GetInt32(reader.GetOrdinal("teacher_id"));

                        if (!teachersDict.TryGetValue(teacherId, out var teacher))
                        {
                            teacher = new Teacher
                            {
                                TeacherId = teacherId,
                                FullName = reader.GetString(reader.GetOrdinal("full_name")),
                                RoomNumber = reader.GetString(reader.GetOrdinal("room_number")),
                                Classes = new List<Class>(),
                                Subjects = new List<Subject>()
                            };
                            teachersDict.Add(teacherId, teacher);
                        }

                        var classId = reader.IsDBNull(reader.GetOrdinal("class_id")) ? -1 : reader.GetInt32(reader.GetOrdinal("class_id"));
                        if (classId != -1 && teacher.Classes.All(c => c.ClassId != classId))
                        {
                            var classObj = new Class
                            {
                                ClassId = classId,
                                ClassNumber = reader.GetString(reader.GetOrdinal("class_number"))
                            };
                            teacher.Classes.Add(classObj);
                        }

                        var subjectId = reader.IsDBNull(reader.GetOrdinal("subject_id")) ? -1 : reader.GetInt32(reader.GetOrdinal("subject_id"));
                        if (subjectId != -1 && teacher.Subjects.All(s => s.SubjectId != subjectId))
                        {
                            var subject = new Subject
                            {
                                SubjectId = subjectId,
                                SubjectName = reader.GetString(reader.GetOrdinal("subject_name"))
                            };
                            teacher.Subjects.Add(subject);
                        }
                    }
                }
            }

            return teachersDict.Values;
        }


        //-------------------------------------------------------------
        public void AddTeacher(string fullName, string roomNumber)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO teacher (full_name, room_number) VALUES (@FullName, @RoomNumber)", connection))
                {
                    command.Parameters.AddWithValue("@FullName", fullName);
                    command.Parameters.AddWithValue("@RoomNumber", roomNumber);
                    command.ExecuteNonQuery();
                }
            }
        }
        //-------------------------------------------------------------
        public void DeleteTeacher(int teacherId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Удаление связей учителя с предметами
                using (var deleteTeacherSubjectCommand = new NpgsqlCommand("DELETE FROM teacher_subject WHERE teacher_id = @TeacherId", connection))
                {
                    deleteTeacherSubjectCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                    deleteTeacherSubjectCommand.ExecuteNonQuery();
                }

                // Удаление связей учителя с классами
                using (var deleteTeacherClassCommand = new NpgsqlCommand("DELETE FROM teacher_class WHERE teacher_id = @TeacherId", connection))
                {
                    deleteTeacherClassCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                    deleteTeacherClassCommand.ExecuteNonQuery();
                }

                // Удаление учителя
                using (var deleteTeacherCommand = new NpgsqlCommand("DELETE FROM teacher WHERE teacher_id = @TeacherId", connection))
                {
                    deleteTeacherCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                    deleteTeacherCommand.ExecuteNonQuery();
                }
            }
        }

        //-------------------------------------------------------------
        //-------------------------------------------------------------
        public void AddSubjectToTeacher(int teacherId, int[] subjectIds)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var deleteCommand = new NpgsqlCommand("DELETE FROM teacher_subject WHERE teacher_id = @TeacherId", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                    deleteCommand.ExecuteNonQuery();
                }

                foreach (var subjectId in subjectIds)
                {
                    using (var insertCommand = new NpgsqlCommand("INSERT INTO teacher_subject (teacher_id, subject_id) VALUES (@TeacherId, @SubjectId)", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                        insertCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }


        //-------------------------------------------------------------
        public void AddClassToTeacher(int teacherId, int[] classIds)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var deleteCommand = new NpgsqlCommand("DELETE FROM teacher_class WHERE teacher_id = @TeacherId", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                    deleteCommand.ExecuteNonQuery();
                }

                foreach (var classId in classIds)
                {
                    using (var insertCommand = new NpgsqlCommand("INSERT INTO teacher_class (teacher_id, class_id) VALUES (@TeacherId, @ClassId)", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                        insertCommand.Parameters.AddWithValue("@ClassId", classId);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }


        //-------------------------------------------------------------
        //-------------------------------------------------------------


        //-------------------------------------------------------------
        public Teacher GetTeacherById(int teacherId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM teacher WHERE teacher_id = @TeacherId", connection))
                {
                    command.Parameters.AddWithValue("@TeacherId", teacherId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Teacher
                            {
                                TeacherId = reader.GetInt32(reader.GetOrdinal("teacher_id")),
                                FullName = reader.GetString(reader.GetOrdinal("full_name")),
                                RoomNumber = reader.GetString(reader.GetOrdinal("room_number"))
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

        //-------------------------------------------------------------


    }
}