using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Question
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string QuestionText { get; set; }
        public string? Hint { get; set; }
        [Required]
        public string AnswerText { get; set; }
        [Required]
        public Guid ThemeId { get; set; }
        [Required]
        public Theme Theme { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public IEnumerable<QuestionOfPackage> QuestionOfPackages { get; set; }
    }
}