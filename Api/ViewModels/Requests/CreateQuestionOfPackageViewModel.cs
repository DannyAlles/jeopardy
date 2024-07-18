using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreateQuestionOfPackageViewModel
    {
        public Guid QuestionId { get; set; }
        [Required]
        public int X { get; set; }
        [Required]
        public int Y { get; set; }
    }
}
