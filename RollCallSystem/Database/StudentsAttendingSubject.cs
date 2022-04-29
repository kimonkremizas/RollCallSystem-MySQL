using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RollCallSystem.Database
{
    [Keyless]
    public partial class StudentsAttendingSubject
    {
        [Column("email")]
        [StringLength(45)]
        public string? Email { get; set; }
        [Column("subjects")]
        public long Subjects { get; set; }
    }
}
