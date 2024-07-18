using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class UpdateProfessorViewModel
    {
        [Required, MaxLength(255)]
        public string FIO { get; set; }
        public string? NewPassword { get; set; }
        public string? NewPasswordRepeat { get; set; }
    }
}
