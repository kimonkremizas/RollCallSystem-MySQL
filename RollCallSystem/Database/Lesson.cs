using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RollCallSystem.Database
{
    [Table("lesson")]
    [Index("CampusId", Name = "lesson_campus_id_fk")]
    [Index("SubjectId", Name = "lesson_subject_id_fk")]
    public partial class Lesson
    {
        public Lesson()
        {
            Students = new HashSet<User>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("subject_id")]
        public int? SubjectId { get; set; }
        [Column("startTime", TypeName = "datetime")]
        public DateTime StartTime { get; set; }
        [Column("code")]
        public int? Code { get; set; }
        [Column("codeTime", TypeName = "datetime")]
        public DateTime? CodeTime { get; set; }
        [Column("campus_id")]
        public int? CampusId { get; set; }

        [ForeignKey("CampusId")]
        [InverseProperty("Lessons")]
        public virtual Campus? Campus { get; set; }
        [ForeignKey("SubjectId")]
        [InverseProperty("Lessons")]
        public virtual Subject? Subject { get; set; }

        [ForeignKey("LessonId")]
        [InverseProperty("Lessons")]
        public virtual ICollection<User> Students { get; set; }
    }
}
