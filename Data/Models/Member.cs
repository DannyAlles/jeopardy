using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Member
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid TeamId { get; set; }
        [Required]
        public Team Team { get; set; }
        [Required, MaxLength(255)]
        public string FIO { get; set; }
    }
}
