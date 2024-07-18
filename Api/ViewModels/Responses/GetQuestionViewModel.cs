using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetQuestionViewModel
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public string? Hint { get; set; }
        public string AnswerText { get; set; }
        public GetThemeListViewModel Theme { get; set; }

    }
}
