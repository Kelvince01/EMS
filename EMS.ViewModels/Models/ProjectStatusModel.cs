using EMS.ViewModels.Infrastructure.ViewModels;

namespace EMS.ViewModels.Models
{
    public class ProjectStatusModel : ObservableObject
    {
        public int Status { get; set; }
        public string Name { get; set; }
    }
}