using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreateQuestionViewModel
    {
        [Required]
        public string QuestionText { get; set; }
        public string? Hint { get; set; }
        [Required]
        public string AnswerText { get; set; }
        [Required]
        public Guid ThemeId { get; set; }
    }
}
