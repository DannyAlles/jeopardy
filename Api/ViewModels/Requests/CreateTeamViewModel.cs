using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreateTeamViewModel
    {
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Score { get; set; }
        public IEnumerable<CreateMemberViewModel> Members { get; set; }

    }
}
