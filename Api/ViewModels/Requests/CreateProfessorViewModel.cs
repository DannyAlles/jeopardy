using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreateProfessorViewModel
    {
        [Required, MaxLength(255)]
        public string FIO { get; set; }
        [Required, MaxLength(255)]
        public string Login { get; set; }
        [Required, MinLength(8)]
        public string Password { get; set; }
    }
}
