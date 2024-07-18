using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid PackageId { get; set; }
        [Required]
        public Package Package { get; set; }
        [Required]
        public Guid ProfessorId { get; set; }
        [Required]
        public Professor Professor { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public IEnumerable<Team> Teams { get; set; }
    }
}
