using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public Session Session { get; set; }
        [Required, MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public int Score { get; set; }
        public IEnumerable<Member> Members { get; set; }
    }
}
