using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RollCallSystem.Database
{
    [Keyless]
    public partial class Studentlist
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("email")]
        [StringLength(45)]
        public string Email { get; set; } = null!;
    }
}
