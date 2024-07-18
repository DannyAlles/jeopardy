using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetQuestionOfPackageListViewModel
    {
        public Guid QuestionId { get; set; }
        public Guid PackageId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
