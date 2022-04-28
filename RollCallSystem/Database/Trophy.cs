using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RollCallSystem.Database
{
    [Table("trophy")]
    [Index("Id", Name = "trophy_id_uindex", IsUnique = true)]
    public partial class Trophy
    {
        public Trophy()
        {
            Students = new HashSet<User>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(45)]
        public string Name { get; set; } = null!;
        [Column("automatic")]
        public bool? Automatic { get; set; }

        [ForeignKey("TrophyId")]
        [InverseProperty("Trophies")]
        public virtual ICollection<User> Students { get; set; }
    }
}
