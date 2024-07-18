using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class GetAuthenticateViewModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
