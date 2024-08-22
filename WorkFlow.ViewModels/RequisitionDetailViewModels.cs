using WorkFlow.Models;

namespace WorkFlow.ViewModels
{
    public class RequisitionDetailViewModels
    {
        public RequisitionHeader RequisitionHeader { get; set; }
        public RequisitionBody RequisitionBody { get; set; }


        public string ApplicationUserId { get; set; }

        public List<RequisitionApproval> RequisitionApproval { get; set; }
        public List<RequisitionSupplement> RequisitionSupplement { get; set; }
        public Category? Categories { get; set; }
        public SubCategory? SubCategories { get; set; }
    }
}
