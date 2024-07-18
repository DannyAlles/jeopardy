using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreateSessionViewModel
    {
        [Required]
        public Guid PackageId { get; set; }
    }
}
