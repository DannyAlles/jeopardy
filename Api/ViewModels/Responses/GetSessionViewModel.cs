using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetSessionViewModel
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public Guid ProfessorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<GetTeamViewModel> Teams { get; set; }
    }
}
