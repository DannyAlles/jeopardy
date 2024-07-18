using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Theme
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(255)]
        public string Title { get; set; }
        public IEnumerable<Question> Questions { get; set; }
    }
}
