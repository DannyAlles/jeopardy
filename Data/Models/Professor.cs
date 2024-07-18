using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Professor
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(255)]
        public string FIO { get; set; }
        [Required, MaxLength(255)]
        public string Login { get; set; }
        [Required, MinLength(8)]
        public string Password { get; set; }
        public Professor? CreatedBy { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
