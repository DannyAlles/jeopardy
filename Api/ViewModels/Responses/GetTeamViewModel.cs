using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetTeamViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
    }
}
