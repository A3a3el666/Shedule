using Microsoft.EntityFrameworkCore;
using Schedule.Models;
using static Dapper.SqlMapper;

namespace Schedule.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Class> Class { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<ClassSubject> ClassSubject { get; set; }
        public DbSet<TeacherClass> TeacherClass { get; set; }
        public DbSet<TeacherSubject> TeacherSubject { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClassSubject>()
                .HasKey(cs => new { cs.ClassId, cs.SubjectId });

            modelBuilder.Entity<TeacherClass>()
                .HasKey(tc => new { tc.TeacherId, tc.ClassId });

            modelBuilder.Entity<TeacherSubject>()
                .HasKey(ts => new { ts.TeacherId, ts.SubjectId });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("class");
                entity.HasKey(e => e.ClassId);

                entity.Property(e => e.ClassId)
                    .HasColumnName("class_id"); 

                entity.Property(e => e.ClassNumber)
                    .HasColumnName("class_number");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subject"); 
                entity.HasKey(e => e.SubjectId);

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject_id");

                entity.Property(e => e.SubjectName)
                    .HasColumnName("subject_name");
            });

            modelBuilder.Entity<ClassSubject>(entity =>
            {
                entity.ToTable("class_subject");
                entity.HasKey(e => new { e.ClassId, e.SubjectId });

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject_id");

                entity.Property(e => e.ClassId)
                    .HasColumnName("class_id");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("teacher");
                entity.HasKey(e => e.TeacherId);

                entity.Property(e => e.TeacherId)
                    .HasColumnName("teacher_id");

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name");

                entity.Property(e => e.RoomNumber)
                    .HasColumnName("room_number");
            });

            modelBuilder.Entity<TeacherClass>(entity =>
            {
                entity.ToTable("teacher_class");
                entity.HasKey(e => new { e.TeacherId, e.ClassId });

                entity.Property(e => e.TeacherId)
                    .HasColumnName("teacher_id");

                entity.Property(e => e.ClassId)
                    .HasColumnName("class_id");
            });

            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.ToTable("teacher_subject"); 
                entity.HasKey(e => new { e.TeacherId, e.SubjectId });

                entity.Property(e => e.TeacherId)
                    .HasColumnName("teacher_id");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject_id");
            });
        }
    }
}
