using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Requests
{
    public class CreateThemeViewModel
    {
        [Required, MaxLength(255)]
        public string Title { get; set; }
    }
}
