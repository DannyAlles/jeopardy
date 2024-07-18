using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetProfessorViewModel
    {
        public Guid Id { get; set; }
        public string FIO { get; set; }
        public string Login { get; set; }
    }
}
