using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetQuestionListViewModel
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public string? Hint { get; set; }
        public string AnswerText { get; set; }
    }
}
