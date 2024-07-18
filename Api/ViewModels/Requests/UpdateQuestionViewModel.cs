using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class UpdateQuestionViewModel
    {
        [Required]
        public string QuestionText { get; set; }
        public string? Hint { get; set; }
        [Required]
        public string AnswerText { get; set; }
       
    }
}
