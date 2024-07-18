using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetThemeListViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
