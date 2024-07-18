using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class UpdateTeamViewModel
    {
        [Required]
        public int Score { get; set; }
    }
}
