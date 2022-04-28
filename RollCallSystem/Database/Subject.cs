using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RollCallSystem.Database
{
    [Table("subject")]
    [Index("Id", Name = "subject_id_uindex", IsUnique = true)]
    [Index("Name", Name = "subject_name_uindex", IsUnique = true)]
    [Index("TeacherId", Name = "subject_teacher_id_fk")]
    public partial class Subject
    {
        public Subject()
        {
            Lessons = new HashSet<Lesson>();
            Students = new HashSet<User>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("teacher_id")]
        public int? TeacherId { get; set; }
        [Column("name")]
        [StringLength(45)]
        public string Name { get; set; } = null!;

        [ForeignKey("TeacherId")]
        [InverseProperty("Subjects")]
        public virtual User? Teacher { get; set; }
        [InverseProperty("Subject")]
        public virtual ICollection<Lesson> Lessons { get; set; }

        [ForeignKey("SubjectId")]
        [InverseProperty("SubjectsNavigation")]
        public virtual ICollection<User> Students { get; set; }
    }
}
