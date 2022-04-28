using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RollCallSystem.Database
{
    [Table("campus")]
    [Index("Id", Name = "campus_id_uindex", IsUnique = true)]
    public partial class Campus
    {
        public Campus()
        {
            Lessons = new HashSet<Lesson>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(45)]
        public string Name { get; set; } = null!;
        [Column("location")]
        [StringLength(100)]
        public string Location { get; set; } = null!;
        [Column("ssid")]
        [StringLength(45)]
        public string? Ssid { get; set; }

        [InverseProperty("Campus")]
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
