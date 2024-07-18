using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Package
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int QuestionsCount { get; set; }
        [Required]
        public Guid CreatedById { get; set; }
        [Required]
        public Professor CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public IEnumerable<QuestionOfPackage> QuestionOfPackages { get; set; }
    }
}
