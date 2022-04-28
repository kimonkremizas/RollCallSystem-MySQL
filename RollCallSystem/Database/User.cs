using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RollCallSystem.Database
{
    [Table("user")]
    [Index("RoleId", Name = "user_role_id_fk")]
    public partial class User
    {
        public User()
        {
            Subjects = new HashSet<Subject>();
            Lessons = new HashSet<Lesson>();
            SubjectsNavigation = new HashSet<Subject>();
            Trophies = new HashSet<Trophy>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("email")]
        [StringLength(45)]
        public string Email { get; set; } = null!;
        [Column("password")]
        [StringLength(45)]
        public string Password { get; set; } = null!;
        [Column("firstName")]
        [StringLength(45)]
        public string FirstName { get; set; } = null!;
        [Column("lastName")]
        [StringLength(45)]
        public string LastName { get; set; } = null!;
        [Column("role_id")]
        public int? RoleId { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("Users")]
        public virtual Role? Role { get; set; }
        [InverseProperty("Teacher")]
        public virtual ICollection<Subject> Subjects { get; set; }

        [ForeignKey("StudentId")]
        [InverseProperty("Students")]
        public virtual ICollection<Lesson> Lessons { get; set; }
        [ForeignKey("StudentId")]
        [InverseProperty("Students")]
        public virtual ICollection<Subject> SubjectsNavigation { get; set; }
        [ForeignKey("StudentId")]
        [InverseProperty("Students")]
        public virtual ICollection<Trophy> Trophies { get; set; }
    }
}
