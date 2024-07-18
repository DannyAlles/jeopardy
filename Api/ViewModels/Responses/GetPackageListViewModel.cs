using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Responses
{
    public class GetPackageListViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }
}
