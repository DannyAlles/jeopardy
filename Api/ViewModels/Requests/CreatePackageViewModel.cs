using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreatePackageViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public int QuestionsCount { get; set; }
        public IEnumerable<CreateQuestionOfPackageViewModel> QuestionOfPackages { get; set; }
    }
}
