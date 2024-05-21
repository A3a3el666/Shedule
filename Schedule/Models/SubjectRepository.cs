using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Schedule.Models;

namespace Schedule.Models
{
    public class SubjectRepository
    {

        private readonly string _connectionString;

        public SubjectRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        //---------------------------------------------------------
        public IEnumerable<Subject> GetAllSubjects()
        {
            var subjects = new List<Subject>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM subject", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var subject = new Subject
                        {
                            SubjectId = reader.GetInt32(reader.GetOrdinal("subject_id")),
                            SubjectName = reader.GetString(reader.GetOrdinal("subject_name"))
                        };
                        subjects.Add(subject);
                    }
                }
            }

            return subjects;
        }
        //---------------------------------------------------------
        public void AddSubject(Subject subject)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO subject (subject_name) VALUES (@SubjectName)", connection))
                {
                    command.Parameters.AddWithValue("@SubjectName", subject.SubjectName);
                    command.ExecuteNonQuery();
                }
            }
        }
        //---------------------------------------------------------
        public void DeleteSubject(int subjectId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Удаление связей предмета с учителями
                using (var deleteTeacherSubjectCommand = new NpgsqlCommand("DELETE FROM teacher_subject WHERE subject_id = @SubjectId", connection))
                {
                    deleteTeacherSubjectCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                    deleteTeacherSubjectCommand.ExecuteNonQuery();
                }

                // Удаление связей предмета с классами
                using (var deleteClassSubjectCommand = new NpgsqlCommand("DELETE FROM class_subject WHERE subject_id = @SubjectId", connection))
                {
                    deleteClassSubjectCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                    deleteClassSubjectCommand.ExecuteNonQuery();
                }

                // Удаление предмета
                using (var deleteSubjectCommand = new NpgsqlCommand("DELETE FROM subject WHERE subject_id = @SubjectId", connection))
                {
                    deleteSubjectCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                    deleteSubjectCommand.ExecuteNonQuery();
                }
            }
        }

        //---------------------------------------------------------
        //---------------------------------------------------------
        //---------------------------------------------------------
    }
}
