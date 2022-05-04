using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RollCallSystem.Database
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Campus> Campuses { get; set; } = null!;
        public virtual DbSet<Lesson> Lessons { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Studentlist> Studentlists { get; set; } = null!;
        public virtual DbSet<StudentsAttendingSubject> StudentsAttendingSubjects { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<Teacherlist> Teacherlists { get; set; } = null!;
        public virtual DbSet<Trophy> Trophies { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Campus>(entity =>
            {
                entity.Property(e => e.Location).HasDefaultValueSql("'Default Address 29N, 2200'");

                entity.Property(e => e.Name).HasDefaultValueSql("'Campus Name'");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasOne(d => d.Campus)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.CampusId)
                    .HasConstraintName("lesson_campus_id_fk");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("lesson_subject_id_fk");
            });

            modelBuilder.Entity<Studentlist>(entity =>
            {
                entity.ToView("studentlist");
            });

            modelBuilder.Entity<StudentsAttendingSubject>(entity =>
            {
                entity.ToView("students_attending_subjects");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("subject_teacher_id_fk");

                entity.HasMany(d => d.Students)
                    .WithMany(p => p.SubjectsNavigation)
                    .UsingEntity<Dictionary<string, object>>(
                        "SubjectStudent",
                        l => l.HasOne<User>().WithMany().HasForeignKey("StudentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("subject_student_student_id_fk"),
                        r => r.HasOne<Subject>().WithMany().HasForeignKey("SubjectId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("subject_student_subject_id_fk"),
                        j =>
                        {
                            j.HasKey("SubjectId", "StudentId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("subject_student");

                            j.HasIndex(new[] { "SubjectId" }, "subject_id");

                            j.HasIndex(new[] { "StudentId" }, "subject_student_student_id_fk");

                            j.IndexerProperty<int>("SubjectId").HasColumnName("subject_id");

                            j.IndexerProperty<int>("StudentId").HasColumnName("student_id");
                        });
            });

            modelBuilder.Entity<Teacherlist>(entity =>
            {
                entity.ToView("teacherlist");
            });

            modelBuilder.Entity<Trophy>(entity =>
            {
                entity.Property(e => e.Name).HasDefaultValueSql("'Default Trophy'");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("user_role_id_fk");

                entity.HasMany(d => d.Lessons)
                    .WithMany(p => p.Students)
                    .UsingEntity<Dictionary<string, object>>(
                        "AttendingStudent",
                        l => l.HasOne<Lesson>().WithMany().HasForeignKey("LessonId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("attending_student_lesson_id_fk"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("StudentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("attending_student_student_id_fk"),
                        j =>
                        {
                            j.HasKey("StudentId", "LessonId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("attending_student");

                            j.HasIndex(new[] { "LessonId" }, "attending_student_lesson_id_fk");

                            j.IndexerProperty<int>("StudentId").HasColumnName("student_id");

                            j.IndexerProperty<int>("LessonId").HasColumnName("lesson_id");
                        });

                entity.HasMany(d => d.Trophies)
                    .WithMany(p => p.Students)
                    .UsingEntity<Dictionary<string, object>>(
                        "StudentTrophy",
                        l => l.HasOne<Trophy>().WithMany().HasForeignKey("TrophyId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("student_trophies_trophy_id_fk"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("StudentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("student_trophies_student_id_fk"),
                        j =>
                        {
                            j.HasKey("StudentId", "TrophyId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("student_trophies");

                            j.HasIndex(new[] { "StudentId" }, "student_id");

                            j.HasIndex(new[] { "TrophyId" }, "student_trophies_trophy_id_fk");

                            j.IndexerProperty<int>("StudentId").HasColumnName("student_id");

                            j.IndexerProperty<int>("TrophyId").HasColumnName("trophy_id");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
