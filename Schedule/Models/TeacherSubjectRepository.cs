using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;

namespace Schedule.Models
{
    public interface ITeacherSubjectRepository
    {
        IEnumerable<TeacherSubject> GetAllTeacherSubjects();
    }

    public class TeacherSubjectRepository : ITeacherSubjectRepository
    {
        private readonly string _connectionString;

        public TeacherSubjectRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<TeacherSubject> GetAllTeacherSubjects()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.Query<TeacherSubject>("SELECT * FROM teacher_subject");
            }
        }
    }
}
