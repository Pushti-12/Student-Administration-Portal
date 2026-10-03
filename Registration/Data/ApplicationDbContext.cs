using Microsoft.EntityFrameworkCore;
using Registration.Models;

namespace Registration.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================
        // TABLES
        // =========================

        public DbSet<User> Users { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<CourseTeacher> CourseTeachers { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Marks> Marks { get; set; }

        public DbSet<Fee> Fees { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================
            // TABLE NAMES
            // =========================

            modelBuilder.Entity<User>()
                .ToTable("users");

            modelBuilder.Entity<Student>()
                .ToTable("students");

            modelBuilder.Entity<Teacher>()
                .ToTable("teachers");

            modelBuilder.Entity<Course>()
                .ToTable("courses");

            modelBuilder.Entity<Subject>()
                .ToTable("subjects");

            modelBuilder.Entity<CourseTeacher>()
                .ToTable("course_teachers");

            modelBuilder.Entity<Attendance>()
                .ToTable("attendance");

            modelBuilder.Entity<Marks>()
                .ToTable("marks");

            modelBuilder.Entity<Fee>()
                .ToTable("fees");


            // =========================
            // USER ROLE
            // =========================

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();


            // =========================
            // USER -> STUDENT
            // 1 : 1
            // =========================

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(
                    s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // USER -> TEACHER
            // 1 : 1
            // =========================

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(
                    t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // LEGACY / PRIMARY
            // TEACHER -> COURSE
            // =========================

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // COURSE -> COURSETEACHER
            // =========================

            modelBuilder.Entity<CourseTeacher>()
                .HasOne(ct => ct.Course)
                .WithMany(c => c.CourseTeachers)
                .HasForeignKey(ct => ct.CourseId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // TEACHER -> COURSETEACHER
            // =========================

            modelBuilder.Entity<CourseTeacher>()
                .HasOne(ct => ct.Teacher)
                .WithMany(t => t.CourseTeachers)
                .HasForeignKey(ct => ct.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);


            // Prevent duplicate assignment
            modelBuilder.Entity<CourseTeacher>()
                .HasIndex(
                    ct => new
                    {
                        ct.CourseId,
                        ct.TeacherId
                    })
                .IsUnique();


            // =========================
            // STUDENT -> COURSE
            // ONE COURSE ONLY
            // =========================

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // COURSE -> SUBJECT
            // 1 : MANY
            // =========================

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Subjects)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);


            // Prevent duplicate subject names
            // inside the same course.
            modelBuilder.Entity<Subject>()
                .HasIndex(
                    s => new
                    {
                        s.CourseId,
                        s.SubjectName
                    })
                .IsUnique();


            // =========================
            // ATTENDANCE
            // =========================

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .Property(a => a.Status)
                .HasConversion<string>();


            // =========================
            // MARKS
            // =========================

            modelBuilder.Entity<Marks>()
                .HasOne(m => m.Student)
                .WithMany(s => s.Marks)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Marks>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Marks)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Marks>()
                .HasOne(m => m.Subject)
                .WithMany(s => s.Marks)
                .HasForeignKey(m => m.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Marks>()
                .Property(m => m.MarksObtained)
                .HasColumnName("marks");


            // =========================
            // FEES
            // =========================

            modelBuilder.Entity<Fee>()
                .HasOne(f => f.Student)
                .WithMany(s => s.Fees)
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Fee>()
                .Property(f => f.Amount)
                .HasPrecision(18, 2);
        }
    }
}