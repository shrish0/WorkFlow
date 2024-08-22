using System.Collections.Generic;
using WorkFlow.Models;

namespace WorkFlow.ViewModels
{
    public class RequisitionFileViewModel
    {
       
            public string RequisitionId { get; set; }
            public string Description { get; set; }
            public List<RequisitionSupplement>? ExistingSupplements { get; set; }
        }

    
}
