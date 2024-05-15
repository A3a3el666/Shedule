using Npgsql;

namespace Schedule.Models
{
    public interface IScheduleRepository
    {
        void SaveSchedule(int classId, Dictionary<string, List<ScheduleEntry>> schedule);
        IEnumerable<Schedule> GetSchedules();
    }

    public class ScheduleRepository : IScheduleRepository
    {
        private readonly string _connectionString;

        public ScheduleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        //---------------------------------------------------------------------

        public void SaveSchedule(int classId, Dictionary<string, List<ScheduleEntry>> schedule)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var day in schedule.Keys)
                    {
                        foreach (var entry in schedule[day])
                        {
                            var command = new NpgsqlCommand(
                                @"INSERT INTO schedules (class_id, day_of_week, subject_id, teacher_id, start_time, end_time)
                          VALUES (@ClassId, @DayOfWeek, @SubjectId, @TeacherId, @StartTime, @EndTime)",
                                connection, transaction);

                            command.Parameters.AddWithValue("@ClassId", classId);
                            command.Parameters.AddWithValue("@DayOfWeek", day);
                            command.Parameters.AddWithValue("@SubjectId", entry.SubjectId);
                            command.Parameters.AddWithValue("@TeacherId", entry.TeacherId);
                            command.Parameters.AddWithValue("@StartTime", entry.StartTime);
                            command.Parameters.AddWithValue("@EndTime", entry.EndTime);

                            // Выводим информацию о добавлении в консоль
                            Console.WriteLine($"Добавлено расписание для класса {classId}, день недели: {day}, предмет: {entry.SubjectId}, учитель: {entry.TeacherId}");

                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        //---------------------------------------------------------------------


        public IEnumerable<Schedule> GetSchedules()
        {
            var schedulesWithDetails = new List<Schedule>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(@"
            SELECT s.schedule_id, c.class_number, s.day_of_week, sub.subject_name, t.full_name, s.start_time, s.end_time
            FROM schedules s
            INNER JOIN class c ON s.class_id = c.class_id
            INNER JOIN subject sub ON s.subject_id = sub.subject_id
            INNER JOIN teacher t ON s.teacher_id = t.teacher_id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        schedulesWithDetails.Add(new Schedule
                        {
                            ScheduleId = reader.GetInt32(reader.GetOrdinal("schedule_id")),
                            ClassName = reader.GetString(reader.GetOrdinal("class_number")),
                            DayOfWeek = reader.GetString(reader.GetOrdinal("day_of_week")),
                            SubjectName = reader.GetString(reader.GetOrdinal("subject_name")),
                            TeacherName = reader.GetString(reader.GetOrdinal("full_name")),
                            StartTime = reader.GetTimeSpan(reader.GetOrdinal("start_time")),
                            EndTime = reader.GetTimeSpan(reader.GetOrdinal("end_time"))
                        });
                    }
                }
            }

            return schedulesWithDetails;
        }

    }

}
