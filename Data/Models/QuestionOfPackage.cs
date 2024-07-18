using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [PrimaryKey(nameof(QuestionId), nameof(PackageId))]
    public class QuestionOfPackage
    {
        public Guid QuestionId { get; set; }
        [Required]
        public Question Question { get; set; }
        public Guid PackageId { get; set; }
        [Required]
        public Package Package { get; set; }
        [Required]
        public int X { get; set; }
        [Required]
        public int Y { get; set; }
    }
}
