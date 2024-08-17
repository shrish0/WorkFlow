using WorkFlow.Models;

namespace WorkFlow.ViewModels
{
    public class RequisitionViewModel
    {
        public RequisitionHeader RequisitionHeader { get; set; }
        public RequisitionBody RequisitionBody { get; set; }

        public List<Category>? Categories { get; set; }
        public List<SubCategory>? SubCategories { get; set; }
    }
}
