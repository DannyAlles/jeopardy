using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreateMemberViewModel
    {
        [Required, MaxLength(255)]
        public string FIO { get; set; }
    }
}
